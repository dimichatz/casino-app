using System.Linq.Expressions;
using AutoMapper;
using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Core.Filters;
using CasinoAppBackend.Data;
using CasinoAppBackend.DTO;
using CasinoAppBackend.Exceptions;
using CasinoAppBackend.Extensions;
using CasinoAppBackend.Models;
using CasinoAppBackend.Repositories;

namespace CasinoAppBackend.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AccountService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Processes a transaction for the specified player by applying the appropriate
        /// business logic based on the transaction type (deposit, withdrawal, bet, win, or bonus).
        /// The player's account is updated accordingly, and a transaction audit record is created.
        /// </summary>
        /// <param name="playerId">
        /// The ID of the player whose account the transaction should be applied to.
        /// This value is provided by the authenticated context (not by the client request).
        /// </param>
        /// <param name="request">
        /// The transaction details, including type, amount, currency, and optional game information.
        /// </param>
        /// <returns>
        /// A <see cref="TransactionReadOnlyDTO"/> representing the completed transaction.
        /// For win transactions, this represents the win entry (tax entries are created internally if needed).
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the player or the player's account cannot be found.
        /// </exception>
        /// <exception cref="EntityForbiddenException">
        /// Thrown when the player is self-excluded.
        /// </exception>
        /// <exception cref="InvalidArgumentException">
        /// Thrown when the transaction amount is less than or equal to zero,
        ///  the transaction type is null or unsupported, or the currency is 
        ///  not the system's default (EUR).
        /// </exception>
        /// <exception cref="InsufficientBalanceException">
        /// Thrown when the account lacks sufficient funds for withdrawals or bets.
        /// </exception>
        /// <exception cref="DomainValidationException">
        /// Thrown when deposit or loss limits (daily/weekly/monthly) are exceeded.
        /// </exception>
        public async Task<TransactionReadOnlyDTO> ProcessTransactionAsync(Guid playerId, TransactionRequestDTO request)
        {
            var account = await _unitOfWork.AccountRepository.GetByPlayerIdAsync(playerId)
                ?? throw new EntityNotFoundException(nameof(Account),
                "Account for player with id: " + playerId + " not found.");

            if (request.TransactionType is null)
                throw new InvalidArgumentException(nameof(request.TransactionType),
                        "Transaction type cannot be null.");

            if (string.IsNullOrWhiteSpace(request.Currency) || 
                !string.Equals(request.Currency, "EUR", StringComparison.Ordinal))
                throw new InvalidArgumentException(nameof(request.Currency),"Currency must be system's default EUR.");

            if (request.Amount <= 0)
                throw new InvalidArgumentException(nameof(request.Amount), "Transaction amount must be greater than zero.");

            var transactionStatus = TransactionStatus.Pending;
            var oldBalance = account.Balance;
            decimal newBalance;
            Transaction transaction = new();

            switch (request.TransactionType)
            {
                case TransactionType.Deposit:
                    var player = await _unitOfWork.PlayerRepository.GetAsync(playerId)
                        ?? throw new EntityNotFoundException(nameof(Account),"Player with id: " + playerId + " not found.");

                    if(!player.IsKycVerified)
                        throw new EntityForbiddenException("Kyc" + nameof(Player), "Non KYC verified players cannot deposit.");

                    if (player.IsSelfExcluded)
                        throw new EntityForbiddenException("SelfExclusion" + nameof(Player), "Self-excluded players cannot deposit.");

                    newBalance = await HandleDepositAsync(account, request);
                    break;
                case TransactionType.Withdraw:
                    newBalance = await HandleWithdrawAsync(playerId, account, request);
                    break;
                case TransactionType.Bet:
                    newBalance = await HandleBetAsync(playerId, account, request);
                    break;
                case TransactionType.Win:
                    transaction = await HandleWinAsync(account, request, oldBalance);
                    _logger.LogInformation("Transaction {TransactionType} completed successfully on account with playerId {PlayerId}." +
                        "TransactionNo: {TransactionNumber}", request.TransactionType, playerId, transaction.TransactionNumber);
                    return _mapper.Map<TransactionReadOnlyDTO>(transaction);
                case TransactionType.Bonus:
                    newBalance = await HandleBonusAsync(account, request);
                    break;
                default:
                    throw new InvalidArgumentException(nameof(request.TransactionType),
                        "Unsupported transaction type.");
            }
            transactionStatus = TransactionStatus.Completed; // No external API currently

            transaction = await CreateTransactionAsync(request,
                accountId: account.Id, oldBalance: oldBalance, newBalance: newBalance, transactionStatus: transactionStatus);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Transaction {TransactionType} completed successfully on account with playerId {PlayerId}." +
                "TransactionNo: {TransactionNumber}", request.TransactionType, playerId, transaction.TransactionNumber);
            return _mapper.Map<TransactionReadOnlyDTO>(transaction);
        }

        /// <summary>
        /// Retrieves all transactions for a specific account, applying optional filters such as
        ///  transaction number, date range, and transaction type.
        /// </summary>
        /// <param name="accountId">The ID of the account whose transactions are retrieved.</param>
        /// <param name="transactionFilterDTO">
        /// The filter object containing optional criteria to narrow down the transactions,
        /// such as transaction number, date range, and type categories (deposits, withdrawals, casino, or other).
        /// </param>
        /// <returns>
        /// A list of <see cref="TransactionReadOnlyDTO"/> objects representing the filtered transactions
        /// for the specified account.
        /// </returns>
        public async Task<List<TransactionReadOnlyDTO>> GetAllTransactionsByAccountIdFilteredAsync(Guid accountId,
            TransactionFilterDTO transactionFilterDTO)
        {
            var predicates = new List<Expression<Func<Transaction, bool>>>();

            if (!string.IsNullOrWhiteSpace(transactionFilterDTO.TransactionNumber) &&
                long.TryParse(transactionFilterDTO.TransactionNumber, out var transactionNumber))
            {
                predicates.Add(t => t.TransactionNumber == transactionNumber);
            }

            if (transactionFilterDTO.DateStart.HasValue)
                predicates.Add(t => t.InsertedAt >= transactionFilterDTO.DateStart.Value);

            if (transactionFilterDTO.DateEnd.HasValue)
                predicates.Add(t => t.InsertedAt <= transactionFilterDTO.DateEnd.Value);

            var selectedTypes = new List<TransactionType>();

            if (transactionFilterDTO.IncludeDeposits == true)
                selectedTypes.Add(TransactionType.Deposit);
            if (transactionFilterDTO.IncludeWithdrawals == true)
                selectedTypes.Add(TransactionType.Withdraw);
            if (transactionFilterDTO.IncludeCasino == true)
            {
                selectedTypes.Add(TransactionType.Bet);
                selectedTypes.Add(TransactionType.Win);
                selectedTypes.Add(TransactionType.Tax);
            }
            if (transactionFilterDTO.IncludeOther == true)
                selectedTypes.Add(TransactionType.Bonus);
                
            if (selectedTypes.Count > 0)
                predicates.Add(t => selectedTypes.Contains(t.TransactionType));

            var transactions = await _unitOfWork.AccountRepository.GetAllTransactionsByAccountIdFilteredAsync(accountId, predicates);
            return _mapper.Map<List<TransactionReadOnlyDTO>>(transactions);
        }

        /// <summary>
        /// Retrieves paginated transactions for a specific account, applying optional filters such as
        ///  transaction number, date range, and transaction type.
        /// </summary>
        /// <param name="accountId">The ID of the account whose transactions are retrieved.</param>
        /// <param name="pageNumber">The current page number for pagination.</param>
        /// <param name="pageSize">The number of records to return per page.</param>
        /// <param name="transactionFilterDTO">
        /// The filter object containing optional criteria to narrow down the transactions,
        /// such as  transaction number, date range, and type categories (deposits, withdrawals, casino, or other).
        /// </param>
        /// <returns>
        /// A <see cref="PaginatedResult{TransactionReadOnlyDTO}"/> containing the filtered transactions
        /// for the specified account, along with pagination metadata.
        /// </returns>
        public async Task<PaginatedResult<TransactionReadOnlyDTO>> GetPaginatedTransactionsByAccountIdFilteredAsync(Guid accountId,
            int pageNumber, int pageSize, TransactionFilterDTO transactionFilterDTO)
        {
            var predicates = new List<Expression<Func<Transaction, bool>>>();

            if (!string.IsNullOrWhiteSpace(transactionFilterDTO.TransactionNumber) &&
                long.TryParse(transactionFilterDTO.TransactionNumber, out var transactionNumber))
            {
                predicates.Add(t => t.TransactionNumber == transactionNumber);
            }

            if (transactionFilterDTO.DateStart.HasValue)
                predicates.Add(t => t.InsertedAt >= transactionFilterDTO.DateStart.Value);

            if (transactionFilterDTO.DateEnd.HasValue)
                predicates.Add(t => t.InsertedAt <= transactionFilterDTO.DateEnd.Value);

            var selectedTypes = new List<TransactionType>();

            if (transactionFilterDTO.IncludeDeposits == true)
                selectedTypes.Add(TransactionType.Deposit);
            if (transactionFilterDTO.IncludeWithdrawals == true)
                selectedTypes.Add(TransactionType.Withdraw);
            if (transactionFilterDTO.IncludeCasino == true)
            {
                selectedTypes.Add(TransactionType.Bet);
                selectedTypes.Add(TransactionType.Win);
                selectedTypes.Add(TransactionType.Tax);
            }
            if (transactionFilterDTO.IncludeOther == true)
                selectedTypes.Add(TransactionType.Bonus);

            if (selectedTypes.Count > 0)
                predicates.Add(t => selectedTypes.Contains(t.TransactionType));

            var paginatedTransactions = await _unitOfWork.AccountRepository
              .GetPaginatedTransactionsByAccountIdFilteredAsync(accountId, pageNumber, pageSize, predicates);

            var transactionDtos = _mapper.Map<List<TransactionReadOnlyDTO>>(paginatedTransactions.Data);

            return new PaginatedResult<TransactionReadOnlyDTO>
            {
                Data = transactionDtos,
                TotalRecords = paginatedTransactions.TotalRecords,
                PageNumber = paginatedTransactions.PageNumber,
                PageSize = paginatedTransactions.PageSize
            };
        }

        public async Task<PlayerBalanceReadOnlyDTO> GetPlayerBalanceAsync(Guid playerId)
        {
            var account = await _unitOfWork.AccountRepository.GetByPlayerIdAsync(playerId)
                ?? throw new EntityNotFoundException(nameof(Account),
                "Account for player with id: " + playerId + " not found.");

            return new PlayerBalanceReadOnlyDTO
            {
                Balance = account.Balance
            };
        }

        /// <summary>
        /// Creates a new transaction record based on the provided request details and account state.
        /// The transaction is generated as part of the transaction processing flow and serves as an audit entry.
        /// </summary>
        /// <param name="request">
        /// The transaction request containing the details such as type, amount, currency, and game information.
        /// </param>
        /// <param name="accountId">The ID of the account associated with the transaction.</param>
        /// <param name="oldBalance">The account balance before the transaction was applied.</param>
        /// <param name="newBalance">The account balance after the transaction was applied.</param>
        /// <param name="transactionStatus">The current status of the transaction (e.g., Failed, Completed).</param>
        /// <returns>
        /// A <see cref="Transaction"/> entity representing the created transaction record.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when a game ID is provided but no corresponding game exists.
        /// </exception>
        private async Task<Transaction> CreateTransactionAsync(TransactionRequestDTO request,
            Guid accountId,decimal oldBalance, decimal newBalance, TransactionStatus transactionStatus)
        {
            string? gameName = null;

            if (request.GameId is not null)
            {
                var game = await _unitOfWork.GameRepository.GetAsync(request.GameId.Value)
                    ?? throw new EntityNotFoundException(nameof(Game),
                "Game with id: " + request.GameId + " not found."); ;
                gameName = game.Name;
            } 

            var transaction = new Transaction
            {
                TransactionType = request.TransactionType!.Value,
                TransactionNumber = await _unitOfWork.TransactionRepository.GenerateTransactionNumberAsync(),
                TransactionStatus = transactionStatus,
                Amount = request.Amount,
                Currency = request.Currency!,
                OldBalance = oldBalance,
                NewBalance = newBalance,
                GameId = request.GameId,
                GameName = gameName,
                GameRoundId = request.GameRoundId,
                AccountId = accountId
            };

            await _unitOfWork.TransactionRepository.AddAsync(transaction);
            return transaction;
        }

        /// <summary>
        /// Handles a deposit transaction by verifying the player's deposit limits and updating their account balance.
        /// If any daily, weekly, or monthly deposit limit is exceeded, a validation exception is thrown.
        /// </summary>
        /// <param name="account">The player's account to which the deposit will be applied.</param>
        /// <param name="request">The transaction request containing the deposit amount and player details.</param>
        /// <returns>
        /// The updated account balance after the deposit is successfully applied.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the player's limit configuration cannot be found.
        /// </exception>
        /// <exception cref="DomainValidationException">
        /// Thrown when the deposit would exceed the player's daily, weekly, or monthly deposit limits,
        /// or the deposit amount is out of range.
        /// <exception cref="SystemConfigurationException">
        /// Thrown when one or more required <c>AppSettings</c> values (min/max deposit amount)
        /// are missing or contain invalid numeric values.
        /// </exception>
        private async Task<decimal> HandleDepositAsync(Account account, TransactionRequestDTO request)
        {
            var minDepositAmountSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("MinDepositAmount")
                ?? throw new SystemConfigurationException("AppSettingsMissing",
                    "Casino settings missing: Min deposit amount must be configured."); ;
            var maxDepositAmountSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("MaxDepositAmount")
                ?? throw new SystemConfigurationException("AppSettingsMissing",
                    "Casino settings missing: Max deposit amount must be configured.");

            if (!decimal.TryParse(minDepositAmountSetting.Value, out var minDepositAmount) ||
                !decimal.TryParse(maxDepositAmountSetting.Value, out var maxDepositAmount))
            {
                throw new SystemConfigurationException("AppSettingsInvalidFormat",
                    "One or more deposit AppSetting values are not valid decimal numbers.");
            }

            if (request.Amount < minDepositAmount || request.Amount > maxDepositAmount)
                throw new DomainValidationException("DepositAmountOutOfRange",
                    "Player with id: " + account.PlayerId + "  exceeded deposit amount range.");

            var playerLimit = await _unitOfWork.PlayerLimitRepository.GetByPlayerIdAsync(account.PlayerId)
                ?? throw new EntityNotFoundException(nameof(PlayerLimit),
                "Limit for player with id: " + account.PlayerId + " not found.");

            var now = DateTimeOffset.UtcNow;

            var dailyDeposits = await _unitOfWork.TransactionRepository
                .GetTotalTransactionAmountByTypeAsync(account.Id, TransactionType.Deposit, now.StartOfDayUtc());

           var weeklyDeposits = await _unitOfWork.TransactionRepository
                .GetTotalTransactionAmountByTypeAsync(account.Id, TransactionType.Deposit, now.StartOfWeekUtc());

           var monthlyDeposits = await _unitOfWork.TransactionRepository
                .GetTotalTransactionAmountByTypeAsync(account.Id, TransactionType.Deposit, now.StartOfMonthUtc());

            ValidateLimits("Deposit", request.Amount, account.PlayerId,
                dailyDeposits, weeklyDeposits, monthlyDeposits,
                playerLimit.DepositDailyLimit, playerLimit.DepositWeeklyLimit, playerLimit.DepositMonthlyLimit);

            account.Balance += request.Amount;
            return account.Balance; 
        }

        /// <summary>
        /// Handles a withdrawal transaction by validating the player's available balance
        /// and deducting the requested amount from the associated account.
        /// </summary>
        /// <param name="playerId">
        /// The ID of the player initiating the withdrawal. Used only for validation messages and auditing,
        /// since the account entity already contains the balance information.
        /// </param>
        /// <param name="account">The player's account from which the withdrawal amount will be deducted.</param>
        /// <param name="request">The transaction request containing the withdrawal amount and currency.</param>
        /// <returns>
        /// The updated account balance after the withdrawal is successfully applied.
        /// </returns>
        /// <exception cref="InsufficientBalanceException">
        /// Thrown when the player's account balance is lower than the requested withdrawal amount.
        /// </exception>
        private static Task<decimal> HandleWithdrawAsync(Guid playerId, Account account, TransactionRequestDTO request)
        {
            if (account.Balance < request.Amount)
                throw new InsufficientBalanceException("Player",
                    "Player with id: " + playerId + " has insufficient balance for withdrawal.");
            account.Balance -= request.Amount;
            return Task.FromResult(account.Balance);
        }

        /// <summary>
        /// Handles a bet transaction by verifying the player's loss limits and available balance.
        /// Uses the provided <paramref name="playerId"/> solely for error reporting.
        /// If placing the bet would exceed the player's daily, weekly, or monthly loss limits,
        /// or if the account balance is insufficient, an exception is thrown.
        /// </summary>
        /// <param name="playerId">The ID of the player performing the bet. Used for validation messages.</param>
        /// <param name="account">The player's account from which the bet amount will be deducted.</param>
        /// <param name="request">The transaction request containing the bet amount and optional game details.</param>
        /// <returns>
        /// The updated account balance after the bet amount is deducted.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the player's loss limit configuration cannot be found.
        /// </exception>
        /// <exception cref="DomainValidationException">
        /// Thrown when the bet would exceed the player's daily, weekly, or monthly loss limits.
        /// </exception>
        /// <exception cref="InsufficientBalanceException">
        /// Thrown when the player's account balance is less than the bet amount.
        /// </exception>
        private async Task<decimal> HandleBetAsync(Guid playerId, Account account, TransactionRequestDTO request)
        {
            var playerLimit = await _unitOfWork.PlayerLimitRepository.GetByPlayerIdAsync(account.PlayerId)
                ?? throw new EntityNotFoundException(nameof(PlayerLimit),
                "Limit for player with id: " + account.PlayerId + " not found.");

            var now = DateTimeOffset.UtcNow;

            var startOfDay = now.StartOfDayUtc();
            var startOfWeek = now.StartOfWeekUtc();
            var startOfMonth = now.StartOfMonthUtc();

            var dailyWins = await _unitOfWork.TransactionRepository
                .GetTotalTransactionAmountByTypeAsync(account.Id, TransactionType.Win, startOfDay);

            var weeklyWins = await _unitOfWork.TransactionRepository
                .GetTotalTransactionAmountByTypeAsync(account.Id, TransactionType.Win, startOfWeek);

            var monthlyWins = await _unitOfWork.TransactionRepository
                .GetTotalTransactionAmountByTypeAsync(account.Id, TransactionType.Win, startOfMonth);

            var dailyBets = await _unitOfWork.TransactionRepository
                .GetTotalTransactionAmountByTypeAsync(account.Id, TransactionType.Bet, startOfDay);

            var weeklyBets = await _unitOfWork.TransactionRepository
                .GetTotalTransactionAmountByTypeAsync(account.Id, TransactionType.Bet, startOfWeek);

            var monthlyBets = await _unitOfWork.TransactionRepository
                .GetTotalTransactionAmountByTypeAsync(account.Id, TransactionType.Bet, startOfMonth);

            var dailyLosses = dailyBets - dailyWins;
            var weeklyLosses = weeklyBets - weeklyWins;
            var monthlyLosses = monthlyBets - monthlyWins;

            ValidateLimits("Loss", request.Amount, account.PlayerId,
                dailyLosses, weeklyLosses, monthlyLosses,
                playerLimit.LossDailyLimit, playerLimit.LossWeeklyLimit, playerLimit.LossMonthlyLimit);

            if (account.Balance < request.Amount)
                throw new InsufficientBalanceException("Player",
                    "Player with id: " + playerId + " has insufficient balance for bet.");
            account.Balance -= request.Amount;
            return account.Balance;
        }

        /// <summary>
        /// Handles a win transaction by updating the player's balance and creating a corresponding transaction record.
        /// A tax may be calculated and applied based on the player's net profit according to
        /// Article 60 of Law 2961/2001 (as amended) — Taxation of Gambling Winnings in Greece:
        /// 0% for net profit up to €100, 15% for €100.01–€500, and 20% for profits above €500.
        /// The method first records the win transaction, and if a tax applies, creates a separate tax transaction.
        /// The returned transaction represents the win before tax deductions.
        /// </summary>
        /// <param name="account">The player's account to which the winnings are applied.</param>
        /// <param name="request">The transaction request containing the win details and amount.</param>
        /// <param name="oldBalance">The account balance before the win transaction was processed.</param>
        /// <returns>
        /// The <see cref="Transaction"/> record representing the win transaction (excluding any tax deductions).
        /// </returns>
        /// <exception cref="InvalidArgumentException">
        /// Thrown when the bet amount is missing from the request.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the corresponding bet transaction for the specified game round cannot be found in the transaction audit table.
        /// </exception>
        private async Task<Transaction> HandleWinAsync(Account account,
            TransactionRequestDTO request, decimal oldBalance)
        {
            if (request.BetAmount is null)
                throw new InvalidArgumentException(nameof(request.BetAmount),
                    "Bet amount must not be null.");

            account.Balance += request.Amount;
            var balanceBeforeTax = account.Balance;

            var winRequest = new TransactionRequestDTO
            {
                GameId = request.GameId,
                GameRoundId = request.GameRoundId,
                TransactionType = TransactionType.Win,
                Amount = request.Amount,
                Currency = request.Currency
            };

            var winTransaction = await CreateTransactionAsync(request: winRequest,
                accountId: account.Id, oldBalance: oldBalance, newBalance: balanceBeforeTax,
                transactionStatus: TransactionStatus.Completed);

            var netProfit = request.Amount - request.BetAmount.Value;
            if (netProfit > 100)
            {
                decimal taxRate = netProfit <= 500 ? 0.15m : 0.20m;
                var taxAmount = netProfit * taxRate;

                account.Balance -= taxAmount;
                var balanceAfterTax = account.Balance;

                var taxRequest = new TransactionRequestDTO
                {
                    GameId = request.GameId,
                    GameRoundId = request.GameRoundId,
                    TransactionType = TransactionType.Tax,
                    Amount = netProfit * taxRate,
                    Currency = request.Currency
                };

                var taxTransaction = await CreateTransactionAsync(request: taxRequest,
                accountId: account.Id, oldBalance: balanceBeforeTax, newBalance: balanceAfterTax,
                transactionStatus: TransactionStatus.Completed);
            }

            await _unitOfWork.SaveAsync();
            return winTransaction;
        }

        /// <summary>
        /// Handles a bonus transaction by adding the specified bonus amount to the player's account balance.
        /// This method is used for system-triggered bonuses such as welcome or promotional rewards.
        /// </summary>
        /// <param name="account">The player's account receiving the bonus.</param>
        /// <param name="request">The transaction request containing the bonus amount and details.</param>
        /// <returns>
        /// The updated account balance after the bonus is applied.
        /// </returns>
        private static Task<decimal> HandleBonusAsync(Account account, TransactionRequestDTO request)
        {
            account.Balance += request.Amount;
            return Task.FromResult(account.Balance);
        }

        /// <summary>
        /// Validates whether the player's daily, weekly, or monthly limits will be exceeded
        /// by a new transaction request. If any limit is surpassed, a <see cref="DomainValidationException"/>
        /// is thrown.
        /// </summary>
        /// <param name="type">The type of limit being validated (e.g., "Deposit" or "Loss").</param>
        /// <param name="requestAmount">The amount of the current transaction request.</param>
        /// <param name="playerId">The unique identifier of the player.</param>
        /// <param name="dailyTotal">The player's total amount so far for the current day.</param>
        /// <param name="weeklyTotal">The player's total amount so far for the current week.</param>
        /// <param name="monthlyTotal">The player's total amount so far for the current month.</param>
        /// <param name="dailyLimit">The player's maximum allowed amount for the day.</param>
        /// <param name="weeklyLimit">The player's maximum allowed amount for the week.</param>
        /// <param name="monthlyLimit">The player's maximum allowed amount for the month.</param>
        /// <exception cref="DomainValidationException">
        /// Thrown when any of the daily, weekly, or monthly limits would be exceeded by the transaction.
        /// </exception>
        private static void ValidateLimits(string type, decimal requestAmount, Guid playerId,
            decimal dailyTotal, decimal weeklyTotal, decimal monthlyTotal,
            decimal dailyLimit, decimal weeklyLimit, decimal monthlyLimit)
        {
            if (dailyTotal + requestAmount > dailyLimit)
                throw new DomainValidationException($"{type}LimitExceeded",
                    $"Player with id: {playerId} exceeded daily {type.ToLower()} limit.");

            if (weeklyTotal + requestAmount > weeklyLimit)
                throw new DomainValidationException($"{type}LimitExceeded",
                    $"Player with id: {playerId} exceeded weekly {type.ToLower()} limit.");

            if (monthlyTotal + requestAmount > monthlyLimit)
                throw new DomainValidationException($"{type}LimitExceeded",
                    $"Player with id: {playerId} exceeded monthly {type.ToLower()} limit.");
        }
    }
}
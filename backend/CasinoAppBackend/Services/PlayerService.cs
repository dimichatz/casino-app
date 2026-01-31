using System.Text.Json;
using AutoMapper;
using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Core.Filters;
using CasinoAppBackend.Data;
using CasinoAppBackend.DTO;
using CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerUpdateFullDetailsPlayerDTO;
using CasinoAppBackend.Exceptions;
using CasinoAppBackend.Helpers;
using CasinoAppBackend.Models;
using CasinoAppBackend.Repositories;
using CasinoAppBackend.Security;
using CasinoAppBackend.Services.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PlayerService> _logger;
        private readonly IAccountService _accountService;
        private readonly IFileStorageService _fileStorageService;

        public PlayerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PlayerService> logger,
            IAccountService accountService, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _accountService = accountService;
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Registers a new player in the system after validating uniqueness of username, email, 
        /// and phone number. Initializes the player's default deposit and loss limits from configured 
        /// <c>AppSettings</c>, creates the player's initial KYC document using the provided 
        /// identification details, creates an associated account in the system currency, 
        /// and applies the default sign-up bonus via the <see cref="AccountService"/>.
        /// </summary>
        /// <param name="request">
        /// The player registration data containing credentials, personal details, and initial 
        /// KYC document information (document type and document number).
        /// </param>
        /// <returns>
        /// A <see cref="PlayerReadOnlyDTO"/> representing the newly created player.
        /// </returns>
        /// <exception cref="EntityAlreadyExistsException">
        /// Thrown when a player with the same username, email, or phone number already exists.
        /// </exception>
        /// <exception cref="MultipleErrorsException">
        /// Thrown when multiple uniqueness violations (for example, both username and email already exist) are detected.
        /// </exception>
        /// <exception cref="SystemConfigurationException">
        /// Thrown when one or more required <c>AppSettings</c> values (deposit/loss limits, system currency, or default bonus)
        /// are missing or contain invalid numeric values.
        /// </exception>
        /// <remarks>
        /// This method:
        /// <list type="bullet">
        /// <item>Validates the uniqueness of username, email, and phone number against existing users.</item>
        /// <item>Retrieves and parses system configuration (<c>AppSettings</c>) for deposit/loss limits,
        /// currency, and default bonus.</item>
        /// <item>Creates new <see cref="User"/>, <see cref="Player"/>, <see cref="KycDocument"/> and <see cref="Account"/> entities
        /// and persists them to the database.</item>
        /// <item>Applies a default sign-up bonus transaction if a nonzero value is configured.</item>
        /// <item>Commits all player creation and bonus operations atomically to ensure consistency.</item>
        /// </list>
        /// </remarks>
        public async Task<PlayerReadOnlyDTO> SignUpPlayerAsync(PlayerSignUpDTO request)
        {
            var user = _mapper.Map<User>(request);
            var player = _mapper.Map<Player>(request);
            var kycDocument = _mapper.Map<KycDocument>(request);
            var errors = new List<ErrorDetail>();

            if (await _unitOfWork.UserRepository.GetByUsernameAsync(user.Username) is not null)
                errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserAlreadyExists",
                    "User with username " + user.Username + " already exists."));

            if (await _unitOfWork.UserRepository.GetByEmailAsync(user.Email) is not null)
                errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserAlreadyExists",
                    "User with email " + user.Email + " already exists."));

            if (await _unitOfWork.UserRepository.GetByPhoneNumberAsync(user.PhoneNumber) is not null)
                errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserAlreadyExists",
                    "User with phone number " + user.PhoneNumber + " already exists."));
            
            if (errors.Count > 0)
            {
                if (errors.Count == 1)
                {
                    var error = errors[0];
                    throw new EntityAlreadyExistsException(error.Code!, error.Message!);
                }
                throw new MultipleErrorsException(errors, "SignUpMultipleErrors",
                                "Multiple errors occurred while signing up the player.");
            }

            var depositDailyAppSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("DepositDailyLimit");
            var depositWeeklyAppSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("DepositWeeklyLimit");
            var depositMonthlyAppSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("DepositMonthlyLimit");
            var lossDailyAppSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("LossDailyLimit");
            var lossWeeklyAppSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("LossWeeklyLimit");
            var lossMonthlyAppSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("LossMonthlyLimit");

            if (depositDailyAppSetting is null || depositWeeklyAppSetting is null || depositMonthlyAppSetting is null ||
                lossDailyAppSetting is null || lossWeeklyAppSetting is null || lossMonthlyAppSetting is null)
            {
                throw new SystemConfigurationException("AppSettingsMissing", 
                    "Casino settings missing: Deposit/Loss limits must be configured.");
            }

            if (!decimal.TryParse(depositDailyAppSetting.Value, out var depositDailyLimit) ||
                !decimal.TryParse(depositWeeklyAppSetting.Value, out var depositWeeklyLimit) ||
                !decimal.TryParse(depositMonthlyAppSetting.Value, out var depositMonthlyLimit) ||
                !decimal.TryParse(lossDailyAppSetting.Value, out var lossDailyLimit) ||
                !decimal.TryParse(lossWeeklyAppSetting.Value, out var lossWeeklyLimit) ||
                !decimal.TryParse(lossMonthlyAppSetting.Value, out var lossMonthlyLimit))
            {
                throw new SystemConfigurationException("AppSettingsInvalidFormat",
                    "One or more limit AppSetting values are not valid decimal numbers.");
            }

            user.UserRole = UserRole.Player;
            user.Password = EncryptionUtil.Encrypt(user.Password);
            await _unitOfWork.UserRepository.AddAsync(user);

            player.User = user;
            player.PlayerLimit.DepositDailyLimit = depositDailyLimit;
            player.PlayerLimit.DepositWeeklyLimit = depositWeeklyLimit;
            player.PlayerLimit.DepositMonthlyLimit = depositMonthlyLimit;
            player.PlayerLimit.LossDailyLimit = lossDailyLimit;
            player.PlayerLimit.LossWeeklyLimit = lossWeeklyLimit;
            player.PlayerLimit.LossMonthlyLimit = lossMonthlyLimit;
            await _unitOfWork.PlayerRepository.AddAsync(player);

            kycDocument.PlayerId = player.Id;
            await _unitOfWork.KycDocumentRepository.AddAsync(kycDocument);

            var systemCurrency = await _unitOfWork.AppSettingRepository.GetByKeyAsync("SystemCurrency")
                ?? throw new SystemConfigurationException("AppSettingsMissing",
                    "Casino settings missing: System currency for all accounts/transactions must be configured.");

            var account = new Account
            {
                Player = player,
                Balance = 0,
                Currency = systemCurrency.Value
            };
            await _unitOfWork.AccountRepository.AddAsync(account);
            await _unitOfWork.SaveAsync();

            var signupBonusAppSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("DefaultSignupBonus")
                ?? throw new SystemConfigurationException("AppSettingsMissing", 
                "Casino settings missing: Default sign-up bonus must be configured.");
            if (!decimal.TryParse(signupBonusAppSetting.Value, out var signupBonus))
                throw new SystemConfigurationException("AppSettingsInvalidFormat",
                    "DefaultSignupBonus must be a valid decimal number.");

            if (signupBonus > 0)
            {
                await _accountService.ProcessTransactionAsync(player.Id, new TransactionRequestDTO
                {
                    TransactionType = TransactionType.Bonus,
                    Amount = signupBonus,
                    Currency = systemCurrency.Value
                });
            }          
            _logger.LogInformation("Player {Username} created successfully.", user.Username);
            return _mapper.Map<PlayerReadOnlyDTO>(player); ;
        }

        /// <summary>
        /// Updates a player's full profile information using the authenticated user's ID.  
        /// Applies only the data segments provided in the request, supporting partial updates
        /// across user details, address information, self-exclusion settings, and player limits.
        /// All changes are validated, audited, and committed atomically to ensure full traceability and data integrity.
        /// </summary>
        /// <param name="userId">
        /// The ID (<see cref="Guid"/>) of the user whose associated player profile is being updated.
        /// </param>
        /// <param name="request">
        /// The DTO containing the player data segments (user details, address, self-exclusion, or limit settings) to update.
        /// </param>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the user, associated player, KYC document, or player limit record cannot be found.
        /// </exception>
        /// <exception cref="EntityAlreadyExistsException">
        /// Thrown when the updated email or phone number already exists on another user account.
        /// </exception>
        /// <exception cref="MultipleErrorsException">
        /// Thrown when multiple uniqueness validation conflicts (for example, both email and phone number) are detected during the update.
        /// </exception>
        /// <exception cref="DomainValidationException">
        /// Thrown when a requested self-exclusion update violates domain rules, such as attempting to shorten an active self-exclusion period
        /// or modifying a permanent self-exclusion.
        /// </exception>
        /// <exception cref="SystemConfigurationException">
        /// Thrown when required <c>AppSettings</c> values for player limit increase delays are missing or incorrectly formatted.
        /// </exception>
        /// <exception cref="DomainConflictException">
        /// Thrown when a limit change is attempted while a pending limit increase exists in the same category (deposit or loss),
        /// preventing further modifications until activation.
        /// </exception>
        /// <remarks>
        /// This method:
        /// <list type="bullet">
        /// <item>Retrieves original (pre-update) snapshots of the <see cref="User"/> and <see cref="Player"/> entities
        /// using <c>AsNoTracking()</c> to support accurate before/after comparisons
        /// for audit generation without running into circular-reference issues.</item>
        /// <item>Validates the uniqueness of critical identifiers such as email and phone number across existing user accounts.</item>
        /// <item>Applies changes only to provided data segments (user details, address, self-exclusion, or limits) 
        /// without affecting untouched fields.</item>
        /// <item>Automatically processes self-exclusion logic, updating start/end dates and disabling user activity for permanent exclusions.</item>
        /// <item>Creates a dedicated self-exclusion audit entry via <see cref="CreatePlayerSelfExclusionAuditAsync(Player)"/> 
        /// whenever a new self-exclusion period is applied.</item>
        /// <item>Handles player limit updates with full regulatory compliance:
        /// applying decreases immediately, deferring increases based on the configured <c>LimitIncreaseDelayDays</c>, 
        /// and preventing any limit modifications in the same category (deposit or loss) while a pending increase is active.</item>
        /// <item>Generates detailed limit change audit entries through <see cref="CreatePlayerLimitAuditAsync(Guid, string, decimal, decimal)"/> 
        /// for each limit modification applied immediately.</item>
        /// <item>Generates <see cref="PlayerDetailsAudit"/> records for modified fields in the <see cref="User"/> and <see cref="Player"/> entities
        /// when the <c>UserDetails</c> or <c>AddressDetails</c> sections are updated.</item>
        /// <item>Commits all updates and audit entries atomically to ensure consistency and traceability.</item>
        /// </list>
        /// </remarks>
        public async Task UpdatePlayerAsync(Guid userId, PlayerUpdateFullDetailsPlayerDTO request)
        {
            var errors = new List<ErrorDetail>();
            var existingUser = await _unitOfWork.UserRepository.GetAsync(userId)
                ?? throw new EntityNotFoundException(nameof(User), "User with id: " + userId + " not found.");
            var existingPlayer = await _unitOfWork.PlayerRepository.GetByUserIdAsync(userId)
                ?? throw new EntityNotFoundException(nameof(Player), "Player with user id: " + userId + " not found.");
            var existingKycDocument = await _unitOfWork.KycDocumentRepository.GetByPlayerIdAsync(existingPlayer.Id)
                ?? throw new EntityNotFoundException(nameof(KycDocument), "KYC Document for player with id: " + existingPlayer.Id + " not found.");
            var existingPlayerLimit = await _unitOfWork.PlayerLimitRepository.GetByPlayerIdAsync(existingPlayer.Id)
                ?? throw new EntityNotFoundException(nameof(PlayerLimit), "Player Limit for player with id: " + existingPlayer.Id + " not found");
            var allPlayerDetailsAudits = new List<PlayerDetailsAudit>();
            var changeUser = existingUser;

            if (request.UserDetails is not null)
            {
                if (request.UserDetails.Email is not null
                        && existingUser.Email != request.UserDetails.Email
                        && await _unitOfWork.UserRepository.GetByEmailAsync(request.UserDetails.Email) is not null)
                    errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserEmailAlreadyExists",
                        "User with email " + request.UserDetails.Email + " already exists."));
                if (request.UserDetails.PhoneNumber is not null
                        && existingUser.PhoneNumber != request.UserDetails.PhoneNumber
                        && await _unitOfWork.UserRepository.GetByPhoneNumberAsync(request.UserDetails.PhoneNumber) is not null)
                    errors.Add(new ErrorDetail(nameof(EntityAlreadyExistsException), "UserPhoneAlreadyExists",
                        "User with phone number " + request.UserDetails.PhoneNumber + " already exists."));

                if (errors.Count > 0)
                {
                    if (errors.Count == 1)
                    {
                        var error = errors.First();
                        throw new EntityAlreadyExistsException(error.Code!, error.Message!);
                    }
                    throw new MultipleErrorsException(errors, "SignUpMultipleErrors",
                                    "Multiple errors occurred while updating the player.");
                }
                
                var oldUser = await _unitOfWork.UserRepository.Query().AsNoTracking().FirstAsync(u => u.Id == existingUser.Id);
                _mapper.Map(request.UserDetails, existingUser);
                var userAudits = EntityAuditBuilder.GetChangedFields(oldUser, existingUser, changeUser, existingPlayer.Id);
                allPlayerDetailsAudits.AddRange(userAudits);
            }

            if (request.AddressDetails is not null)
            {
                var oldPlayer = await _unitOfWork.PlayerRepository.Query().AsNoTracking().FirstAsync(p => p.Id == existingPlayer.Id);
                _mapper.Map(request.AddressDetails, existingPlayer);
                var playerAudits = EntityAuditBuilder.GetChangedFields(oldPlayer, existingPlayer, changeUser, existingPlayer.Id);
                allPlayerDetailsAudits.AddRange(playerAudits);
            }

            if (request.SelfExclusionDetails is not null &&
                request.SelfExclusionDetails.SelfExclusionPeriod is not null)
            {
                var now = DateTime.UtcNow;

                if (existingPlayer.IsSelfExcluded && existingPlayer.SelfExclusionEnd == null)
                {
                    throw new DomainValidationException("PlayerPermanentExclusion",
                        "Permanent self-exclusion cannot be modified.");
                }

                if (request.SelfExclusionDetails.SelfExclusionPeriod != SelfExclusionPeriod.Permanent &&
                    existingPlayer.IsSelfExcluded)
                {
                    var requestedEndDate =
                        now.AddDays((double)request.SelfExclusionDetails.SelfExclusionPeriod);

                    if (existingPlayer.SelfExclusionEnd > requestedEndDate)
                    {
                        throw new DomainValidationException("PlayerPartialExclusion",
                            "Self-exclusion can only be extended and cannot be shortened.");
                    }
                }

                _mapper.Map(request.SelfExclusionDetails, existingPlayer);

                if (request.SelfExclusionDetails.SelfExclusionPeriod == SelfExclusionPeriod.Permanent)
                {
                    existingPlayer.IsSelfExcluded = true;
                    existingPlayer.SelfExclusionEnd = null;
                    existingPlayer.User.IsActive = false;
                }
                else
                {
                    var endDate =
                        now.AddDays((double)request.SelfExclusionDetails.SelfExclusionPeriod);

                    if (!existingPlayer.IsSelfExcluded)
                    {
                        existingPlayer.IsSelfExcluded = true;
                        existingPlayer.SelfExclusionStart = now;
                    }

                    existingPlayer.SelfExclusionEnd = endDate;
                }

                await CreatePlayerSelfExclusionAuditAsync(existingPlayer);
            }


            if (request.LimitDetails is not null)
            {
                var delayDaysAppSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("LimitIncreaseDelayDays")
                    ?? throw new SystemConfigurationException("AppSettingsMissing",
                            "Casino settings missing: Delay period for player's limit amount increase must be configured.");
                if (!int.TryParse(delayDaysAppSetting.Value, out var delayDays))
                    throw new SystemConfigurationException("AppSettingsInvalidFormat",
                        "Delay period for player's limit amount increase must be a valid integer number.");

                var oldPlayerLimit = new PlayerLimit
                {
                    DepositDailyLimit = existingPlayerLimit.DepositDailyLimit,
                    DepositWeeklyLimit = existingPlayerLimit.DepositWeeklyLimit,
                    DepositMonthlyLimit = existingPlayerLimit.DepositMonthlyLimit,
                    LossDailyLimit = existingPlayerLimit.LossDailyLimit,
                    LossWeeklyLimit = existingPlayerLimit.LossWeeklyLimit,
                    LossMonthlyLimit = existingPlayerLimit.LossMonthlyLimit
                };

                foreach (var property in typeof(PlayerUpdateLimitDetailsPlayerDTO).GetProperties())
                {
                    var intValue = (int?)property.GetValue(request.LimitDetails);
                    if (intValue is null)
                        continue;

                    // Determine category of the property (Deposit or Loss)
                    bool isDeposit = property.Name.StartsWith("Deposit", StringComparison.OrdinalIgnoreCase);
                    bool isLoss = property.Name.StartsWith("Loss", StringComparison.OrdinalIgnoreCase);

                    // Check if any pending limit exists in the same category
                    bool isCategoryPending =
                        (isDeposit && (
                            existingPlayerLimit.PendingDepositDailyLimit != null ||
                            existingPlayerLimit.PendingDepositWeeklyLimit != null ||
                            existingPlayerLimit.PendingDepositMonthlyLimit != null
                        ))
                        ||
                        (isLoss && (
                            existingPlayerLimit.PendingLossDailyLimit != null ||
                            existingPlayerLimit.PendingLossWeeklyLimit != null ||
                            existingPlayerLimit.PendingLossMonthlyLimit != null
                        ));

                    if (isCategoryPending)
                    {
                        throw new DomainConflictException(
                            "PlayerLimitPending",
                            property.Name + " cannot be changed while a pending limit increase exists."
                        );
                    }

                    decimal newValue = intValue.Value;

                    var oldValueProperty = typeof(PlayerLimit).GetProperty(property.Name);
                    var oldValue = (decimal?)oldValueProperty?.GetValue(oldPlayerLimit);

                    if (oldValue is null)
                        continue;

                    if (newValue > oldValue)
                    {
                        var pendingProperty = typeof(PlayerLimit).GetProperty($"Pending{property.Name}");
                        var pendingStartProperty = typeof(PlayerLimit).GetProperty($"Pending{property.Name}Start");

                        pendingProperty?.SetValue(existingPlayerLimit, newValue);
                        pendingStartProperty?.SetValue(existingPlayerLimit, DateTimeOffset.UtcNow.AddDays((double)delayDays));
                    }
                    else
                    {
                        var limitProperty = typeof(PlayerLimit).GetProperty(property.Name);
                        limitProperty?.SetValue(existingPlayerLimit, (decimal)newValue);
                        await CreatePlayerLimitAuditAsync(existingPlayer.Id, property.Name, oldValue.Value, newValue);
                    }
                }
            }
            if (allPlayerDetailsAudits.Count > 0)
                await _unitOfWork.PlayerDetailsAuditRepository.AddRangeAsync(allPlayerDetailsAudits);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Player {Username} updated successfully.", existingUser.Username);
        }

        /// <summary>
        /// Changes the password of the authenticated user after validating the current password
        /// and ensuring the new password is not the same as the old one.
        /// </summary>
        /// <param name="userId">The ID of the user whose password is being changed.</param>
        /// <param name="request">The DTO containing the current and new password values.</param>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the user with the specified ID does not exist.
        /// </exception>
        /// <exception cref="DomainValidationException">
        /// Thrown when the cur-rent password is incorrect or when the new password matches the current one.
        /// </exception>
        public async Task ChangePasswordAsync(Guid userId, UserChangePasswordDTO request)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(userId)
                ?? throw new EntityNotFoundException(nameof(User), "User with id: " + userId + " not found.");

            if (!EncryptionUtil.IsValidPassword(request.CurrentPassword!, user.Password))
                throw new DomainValidationException(nameof(request.CurrentPassword), "Current password is incorrect.");
            if (EncryptionUtil.IsValidPassword(request.NewPassword!,user.Password))
                throw new DomainValidationException(nameof(request.NewPassword), "New password must not be the same as current password.");

            user.Password = EncryptionUtil.Encrypt(request.NewPassword!);
            await _unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Retrieves a single player using the associated user's ID.
        /// </summary>
        /// <param name="userId">The ID of the user associated with the player.</param>
        /// <returns>
        /// A <see cref="PlayerFullDetailsReadOnlyDTO"/> containing the player's details.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when no player is found for the specified user ID.
        /// </exception>
        public async Task<PlayerFullDetailsReadOnlyDTO> GetPlayerByUserIdAsync(Guid userId)
        {
            var player = await _unitOfWork.PlayerRepository.GetByUserIdAsync(userId)
                ?? throw new EntityNotFoundException(nameof(Player), "Player with user id: " + userId + " not found.");
            return _mapper.Map<PlayerFullDetailsReadOnlyDTO>(player);
        }

        /// <summary>
        /// Retrieves a player and all related profile data using the associated user's ID.
        /// This includes the player entity itself together with related User, KYC Document
        /// and PlayerLimit information loaded through the repository.
        /// </summary>
        /// <param name="userId">The ID of the user associated with the player.</param>
        /// <returns>
        /// A <see cref="PlayerFullDetailsReadOnlyDTO"/> containing the player's details.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when no player is found for the specified user ID.
        /// </exception>
        public async Task<PlayerFullDetailsReadOnlyDTO> GetPlayerFullDetailsByUserIdAsync(Guid userId)
        {
            var player = await _unitOfWork.PlayerRepository.GetFullDetailsByUserIdAsync(userId)
                ?? throw new EntityNotFoundException(nameof(Player), "Player with user id: " + userId + " not found.");
            return _mapper.Map<PlayerFullDetailsReadOnlyDTO>(player);
        }

        /// <summary>
        /// Retrieves all account transactions for the authenticated player (identified by user ID),
        /// applying optional filters such as transaction number, date range, or transaction type.
        /// </summary>
        /// <param name="userId">
        /// The ID (<see cref="Guid"/>) of the user whose associated player's transactions are being retrieved.
        /// </param>
        /// <param name="filters">
        /// The filter criteria used to narrow the returned transactions (transaction number, date range,
        /// and transaction type categories).
        /// </param>
        /// <returns>
        /// A list of <see cref="TransactionReadOnlyDTO"/> representing the filtered transactions
        /// for the player's account.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when either the player associated with the given user ID, or the player's account,
        /// cannot be found.
        /// </exception>
        public async Task<List<TransactionReadOnlyDTO>> GetTransactionsByUserIdAsync(Guid userId, TransactionFilterDTO filters)
        {
            var player = await _unitOfWork.PlayerRepository.GetByUserIdAsync(userId)
                ?? throw new EntityNotFoundException(nameof(Player), "Player with user id: " + userId + " not found.");
            var account = await _unitOfWork.AccountRepository.GetByPlayerIdAsync(player.Id)
                ?? throw new EntityNotFoundException(nameof(Account), "Account with player id: " + player.Id + " not found.");

            return await _accountService.GetAllTransactionsByAccountIdFilteredAsync(account.Id, filters);
        }

        /// <summary>
        /// Retrieves paginated account transactions for the authenticated player (identified by user ID),
        /// applying optional filters such as transaction number, date range, or transaction type.
        /// </summary>
        /// <param name="userId">
        /// The ID (<see cref="Guid"/>) of the user whose associated player's transactions are being retrieved.
        /// </param>
        /// <param name="pageNumber">The page number for the paginated result.</param>
        /// <param name="pageSize">The number of records to include per page.</param>
        /// <param name="filters">
        /// The filter criteria used to refine the returned transactions (transaction number, date range,
        /// and transaction type categories).
        /// </param>
        /// <returns>
        /// A <see cref="PaginatedResult{TransactionReadOnlyDTO}"/> containing the filtered transactions
        /// for the player's account along with pagination metadata.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when either the player associated with the given user ID, or the player's account,
        /// cannot be found.
        /// </exception>
        public async Task<PaginatedResult<TransactionReadOnlyDTO>> GetPaginatedTransactionsByUserIdAsync(Guid userId,
            int pageNumber, int pageSize, TransactionFilterDTO filters)
        {
            var player = await _unitOfWork.PlayerRepository.GetByUserIdAsync(userId)
                ?? throw new EntityNotFoundException(nameof(Player), "Player with user id: " + userId + " not found.");
            var account = await _unitOfWork.AccountRepository.GetByPlayerIdAsync(player.Id)
                ?? throw new EntityNotFoundException(nameof(Account), "Account with player id: " + player.Id + " not found.");

            return await _accountService.GetPaginatedTransactionsByAccountIdFilteredAsync(account.Id, pageNumber, pageSize, filters);
        }

        /// <summary>
        /// Uploads or replaces the player's KYC document attachment.
        /// Validates the file type, size, and content type, uploads the file to Azure Blob Storage,
        /// updates the <see cref="Attachment"/> metadata, and resets the KYC status to pending.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user performing the upload.
        /// </param>
        /// <param name="file">
        /// The file uploaded by the player, containing the KYC document image or PDF.
        /// </param>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the player or the associated KYC document cannot be found.
        /// </exception>
        /// <exception cref="BadRequestException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item>The KYC is already approved and cannot be modified.</item>
        /// <item>The uploaded file is missing or empty.</item>
        /// <item>The file extension is not one of the allowed types (.jpg, .jpeg, .png, .pdf).</item>
        /// <item>The file MIME type does not match the permitted content types.</item>
        /// <item>The file exceeds the maximum allowed size (5MB) or is suspiciously small.</item>
        /// </list>
        /// </exception>
        /// <remarks>
        /// This method performs the following operations:
        /// <list type="number">
        /// <item>Loads the player and associated <see cref="KycDocument"/> using the provided user ID.</item>
        /// <item>Ensures that approved KYC documents cannot be replaced.</item>
        /// <item>Validates the uploaded file for type, content type, and size constraints.</item>
        /// <item>Generates a unique blob name for secure cloud storage.</item>
        /// <item>Deletes any previously uploaded attachment (both database record and blob file).</item>
        /// <item>Uploads the new file to Azure Blob Storage through <see cref="IFileStorageService"/>.</item>
        /// <item>Creates a new <see cref="Attachment"/> entry linked to the player's KYC document.</item>
        /// <item>Resets the KYC review state to pending and clears previous verification metadata.</item>
        /// <item>Commits all changes atomically via the unit of work.</item>
        /// </list>
        /// </remarks>
        public async Task UploadKycAttachmentAsync(Guid userId, IFormFile file)
        {
            var player = await _unitOfWork.PlayerRepository.GetByUserIdAsync(userId)
                ?? throw new EntityNotFoundException(nameof(Player), "Player with user id: " + userId + " not found.");

            var kycDocument = await _unitOfWork.KycDocumentRepository.GetByPlayerIdAsync(player.Id)
                ?? throw new EntityNotFoundException(nameof(KycDocument), "KYC document for player id: " + player.Id + " not found.");

            if (kycDocument.KycStatus == KycStatus.Approved)
                throw new BadRequestException("KycAlreadyApproved", "Your KYC is already approved. You cannot upload new documents.");

            if (file == null || file.Length == 0)
                throw new BadRequestException("InvalidFile", "A valid file must be uploaded.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new BadRequestException("InvalidFileType", "Only .jpg, .jpeg, .png, and .pdf files are allowed.");

            var allowedContentTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "application/pdf"
            };

            if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
                throw new BadRequestException("InvalidContentType", "The uploaded file's content type is not permitted.");


            const long maxSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxSize)
                throw new BadRequestException("FileTooLarge", "The file must be smaller than 5MB.");

            if (file.Length < 1024) // 1 KB minimum sanity check
                throw new BadRequestException("FileTooSmall", "The file appears to be invalid or empty.");

            var blobName = Guid.NewGuid().ToString() + extension;


            var existingAttachment = await _unitOfWork.AttachmentRepository.GetByKycDocumentIdAsync(kycDocument.Id);

            if (existingAttachment != null)
            {
                await _fileStorageService.DeleteAsync(existingAttachment.BlobName);
                _unitOfWork.AttachmentRepository.Remove(existingAttachment);
            }

            using (var stream = file.OpenReadStream())
            {
                await _fileStorageService.UploadAsync(stream, blobName, file.ContentType);
            }

            var attachment = new Attachment
            {
                FileName = file.FileName,
                BlobName = blobName,
                ContentType = file.ContentType,
                Extension = extension,
                KycDocumentId = kycDocument.Id
            };

            await _unitOfWork.AttachmentRepository.AddAsync(attachment);

            kycDocument.KycStatus = KycStatus.Pending;
            kycDocument.KycCheckDate = null;
            kycDocument.KycCheckedBy = null;
            kycDocument.ExpireDate = null;

            //--------------------------------------------------
            //      UNTIL ADMIN FRONT END IS DEVELOPED

            player.IsKycVerified = true;
            //--------------------------------------------------

            await _unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Creates an audit entry recording a player's self-exclusion event, including start and end dates,
        /// exclusion period, and the associated player ID.
        /// </summary>
        /// <param name="player">
        /// The <see cref="Player"/> entity containing self-exclusion details such as start date,
        /// end date, and exclusion period.
        /// </param>
        /// <remarks>
        /// This method is automatically invoked from <see cref="UpdatePlayerAsync"/> whenever a player
        /// initiates or updates a self-exclusion period. It generates a <see cref="PlayerSelfExclusionAudit"/>
        /// record that logs the exclusion duration and ensures traceability for responsible gaming compliance.
        /// </remarks>
        private async Task CreatePlayerSelfExclusionAuditAsync(Player player)
        {
            var audit = new PlayerSelfExclusionAudit
            {
                SelfExclusionStart = player.SelfExclusionStart,
                SelfExclusionEnd = player.SelfExclusionEnd,
                SelfExclusionPeriod = player.SelfExclusionPeriod,
                PlayerId = player.Id
            };
            await _unitOfWork.PlayerSelfExclusionAuditRepository.AddAsync(audit);
        }

        /// <summary>
        /// Creates an audit entry when a player's limit value changes, recording both old and new amounts.
        /// Executed instantly when the limit amount decreases; increases may be scheduled for delayed application.
        /// </summary>
        /// <param name="playerId">
        /// The ID (<see cref="Guid"/>) of the player whose limit was modified.
        /// </param>
        /// <param name="fieldName">
        /// The name of the limit field that changed (e.g., deposit, loss, or wager limit).
        /// </param>
        /// <param name="oldLimit">
        /// The previous limit value before the update.
        /// </param>
        /// <param name="newLimit">
        /// The new limit value after the update.
        /// </param>
        /// <remarks>
        /// This method is invoked automatically from <see cref="UpdatePlayerAsync"/> whenever a player's limit is updated.
        /// It creates a <see cref="PlayerLimitAudit"/> record capturing the field name, old and new values,
        /// and the associated player identifier for traceability and compliance auditing.
        /// </remarks>
        private async Task CreatePlayerLimitAuditAsync(Guid playerId, string fieldName, decimal oldLimit, decimal newLimit )
        {
            var audit = new PlayerLimitAudit
            {
                FieldName = fieldName,
                OldLimit = oldLimit,
                NewLimit = newLimit,
                PlayerId = playerId
            };
            await _unitOfWork.PlayerLimitAuditRepository.AddAsync(audit);
        }
    }
}
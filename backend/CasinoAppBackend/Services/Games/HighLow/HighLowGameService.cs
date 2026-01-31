using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Core.Enums.Games.LowHigh;
using CasinoAppBackend.Data;
using CasinoAppBackend.DTO;
using CasinoAppBackend.DTO.Games.HighLow;
using CasinoAppBackend.Exceptions;
using CasinoAppBackend.Repositories;
using CasinoAppBackend.Services.Access;

namespace CasinoAppBackend.Services.Games.HighLow
{
    public class HighLowGameService : IHighLowGameService
    {
        private readonly ILogger<HighLowGameService> _logger;
        private readonly IHighLowGameIntegrationService _integrationService;
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlayerAccessValidator _playerAccessValidator;

        public HighLowGameService(ILogger<HighLowGameService> logger,
            IHighLowGameIntegrationService integrationService,
            IAccountService accountService, IUnitOfWork unitOfWork,
            IPlayerAccessValidator playerAccessValidator)
        {
            _logger = logger;
            _integrationService = integrationService;
            _accountService = accountService;
            _unitOfWork = unitOfWork;
            _playerAccessValidator = playerAccessValidator;
        }

        /// <summary>
        /// Starts a new HighLow game session for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user initiating the game session.</param>
        /// <returns>
        /// A <see cref="StartGameResponseDTO"/> containing details about the newly created external game session.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown if the player does not exist or the HighLow game configuration cannot be found.
        /// </exception>
        /// <exception cref="EntityForbiddenException">
        /// Thrown if the player is inactive, not KYC-verified, or self-excluded.
        /// </exception>
        /// <exception cref="ExternalServiceFailedException">
        /// Thrown when the external HighLow Game API returns an unsuccessful response.
        /// </exception>
        /// <exception cref="ExternalServiceInvalidResponseException">
        /// Thrown when the external API returns an invalid or empty response.
        /// </exception>
        public async Task<StartGameResponseDTO> StartGameAsync(Guid userId)
        {
            _logger.LogInformation("Starting HighLow game for UserId: {UserId}", userId);

            _ = await ValidatePlayer(userId);

            var game = await ValidateGame();

            var response = await _integrationService.StartGameAsync(userId);

            var session = new GameSession
            {
                ExternalSessionId = response.SessionId,
                UserId = userId,
                GameId = game.Id,
                GameStatus = GameStatus.Active
            };

            await _unitOfWork.GameSessionRepository.AddAsync(session);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("HighLow session started for UserId: {UserId} with SessionId: {SessionId}",
                userId, response.SessionId);
            return response;
        }

        /// <summary>
        /// Starts a new round within an active HighLow game session.
        /// Processes the player's bet with responsible gambling limit validation,
        /// calls the external game API to resolve the round,
        /// and applies any win payouts returned from the external game.
        /// </summary>
        /// <param name="userId">The ID of the user playing the round.</param>
        /// <param name="request">The round request payload containing session ID and bet information.</param>
        /// <returns>
        /// A <see cref="StartRoundResultResponseDTO"/> containing the outcome of the round.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown if the player, game, or game session cannot be found.
        /// </exception>
        /// <exception cref="EntityForbiddenException">
        /// Thrown if the player is not permitted to play or if the session is not active.
        /// </exception>
        /// <exception cref="SystemConfigurationException">
        /// Thrown when required system currency configuration is missing.
        /// </exception>
        /// <exception cref="ExternalServiceFailedException">
        /// Thrown when the external HighLow Game API returns an unsuccessful response.
        /// </exception>
        /// <exception cref="ExternalServiceInvalidResponseException">
        /// Thrown when the external API returns an invalid or empty response.
        /// </exception>
        public async Task<StartRoundResultResponseDTO> StartRoundAsync(Guid userId, StartRoundRequestDTO request)
        {
            _logger.LogInformation("Starting HighLow round for UserId: {UserId} and SessionId: {SessionId}",
                userId, request.SessionId);

            var player = await ValidatePlayer(userId);

            var game = await ValidateGame();

            _ = await ValidateSession(request.SessionId, userId);

            var appSetting = await _unitOfWork.AppSettingRepository.GetByKeyAsync("SystemCurrency")
                ?? throw new SystemConfigurationException("AppSettingsMissing",
                    "Casino settings missing: System currency for all accounts/transactions must be configured.");

            try
            {
                _ = await _accountService.ProcessTransactionAsync(player.Id, new TransactionRequestDTO
                {
                    TransactionType = TransactionType.Bet,
                    GameId = game.Id,
                    GameRoundId = null,
                    Amount = request.BetAmount,
                    Currency = appSetting.Value
                });
            }
            catch (DomainValidationException ex) 
            when (ex.Code == "LossLimitExceeded")
            {
                var period =
                    ex.Message.Contains("daily") ? "daily" :
                    ex.Message.Contains("weekly") ? "weekly" :
                    ex.Message.Contains("monthly") ? "monthly" :
                    "unknown";

                throw new EntityForbiddenException(
                    "LossLimitExceeded",
                    period
                );
            }


            var response = await _integrationService.StartRoundAsync(request);

            if (response.IsWin)
            {
                _ = await _accountService.ProcessTransactionAsync(player.Id, new TransactionRequestDTO
                {
                    TransactionType = TransactionType.Win,
                    GameId = game.Id,
                    GameRoundId = response.RoundNumber,
                    Amount = response.WinAmount,
                    BetAmount = request.BetAmount,
                    Currency = appSetting.Value
                });
            }

            _logger.LogInformation("HighLow round started for SessionId: {SessionId}, Round: {RoundNumber}",
                request.SessionId, response.RoundNumber);
            return response;
        }

        /// <summary>
        /// Ends a HighLow session. After validating the 
        /// player, game, and session, it sends an acknowledgment request to the external 
        /// HighLow Game API, updates its own session state to <see cref="GameStatus.Terminated"/>,
        /// and returns the final game status to the client.
        /// </summary>
        /// <param name="userId">The ID of the user ending the session.</param>
        /// <param name="sessionId">The ID of the HighLow session to end.</param>
        /// <returns>
        /// An <see cref="EndSessionResponseDTO"/> containing the final
        /// session status and the external service acknowledgment.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown if the player, game, or session cannot be found.
        /// </exception>
        /// <exception cref="EntityForbiddenException">
        /// Thrown if the session does not belong to the user or is not active.
        /// </exception>
        /// <exception cref="ExternalServiceFailedException">
        /// Thrown when the external HighLow Game API returns an unsuccessful response.
        /// </exception>
        /// <exception cref="ExternalServiceInvalidResponseException">
        /// Thrown when the external API returns an invalid or empty response.
        /// </exception>
        public async Task<EndSessionResponseDTO> EndSessionAsync(Guid userId, Guid sessionId)
        {
            _logger.LogInformation("Ending HighLow session for UserId: {UserId} with SessionId: {SessionId}",
                userId, sessionId);

            _ = await ValidatePlayer(userId);

            _ = await ValidateGame();

            var session = await ValidateSession(sessionId, userId);

            var response = await _integrationService.EndSessionAsync(userId, sessionId);
            response.GameStatus = GameStatus.Terminated;

            session.GameStatus = GameStatus.Terminated;
            session.EndedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Session {SessionId} ended successfully for UserId: {UserId}",
                sessionId, userId);
            return response;
        }

        /// <summary>
        /// Times out a HighLow session. After validating the 
        /// player, game, and session, it sends an acknowledgment request to the external 
        /// HighLow Game API, updates its own session state to <see cref="GameStatus.Timeout"/>,
        /// and returns the final game status to the client.
        /// </summary>
        /// <param name="userId">The ID of the user associated with the session.</param>
        /// <param name="sessionId">The ID of the session being timed out.</param>
        /// <returns>
        /// A <see cref="EndSessionResponseDTO"/> containing the final
        /// session status and the external service acknowledgment.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown if the player, game, or session cannot be found.
        /// </exception>
        /// <exception cref="EntityForbiddenException">
        /// Thrown if the session does not belong to the user or is not active.
        /// </exception>
        /// <exception cref="ExternalServiceFailedException">
        /// Thrown when the external HighLow Game API returns an unsuccessful response.
        /// </exception>
        /// <exception cref="ExternalServiceInvalidResponseException">
        /// Thrown when the external API returns an invalid or empty response.
        /// </exception>
        public async Task<EndSessionResponseDTO> TimeoutSessionAsync(Guid userId, Guid sessionId)
        {
            _logger.LogInformation("Timing out HighLow session for UserId: {UserId} with SessionId: {SessionId}",
                userId, sessionId);

            await ValidatePlayer(userId);

            _ = await ValidateGame();

            var session = await ValidateSession(sessionId, userId);

            var response = await _integrationService.TimeoutSessionAsync(userId, sessionId);
            response.GameStatus = GameStatus.Timeout;

            session.GameStatus = GameStatus.Timeout;
            session.EndedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Session {SessionId} timed out for UserId: {UserId}",
                userId, sessionId);
            return response;
        }

        /// <summary>
        /// Retrieves the HighLow game configuration and the player's current account balance.
        /// </summary>
        /// <param name="userId">The ID of the authenticated user requesting the configuration.</param>
        /// <returns>
        /// A <see cref="ConfigResponseDTO"/> containing the minimum and maximum allowed bet and
        /// the player's current account balance.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the player, account, or HighLow game configuration cannot be found.
        /// </exception>
        public async Task<ConfigResponseDTO> GetConfigAsync(Guid userId)
        {
            var player = await _unitOfWork.PlayerRepository.GetByUserIdAsync(userId)
                ?? throw new EntityNotFoundException(nameof(Player), "Player with user id: " + userId + " not found.");
            var account = await _unitOfWork.AccountRepository.GetByPlayerIdAsync(player.Id)
                ?? throw new EntityNotFoundException(nameof(Account), "Account with player id: " + player.Id + " not found.");
            var game = await _unitOfWork.GameRepository.GetByCodeAsync("highlow")
                ?? throw new EntityNotFoundException(nameof(Game), "Game with code: \"highlow\" not found");

            return new ConfigResponseDTO
            {
                Balance = account.Balance,
                MinBet = game.MinBet,
                MaxBet = game.MaxBet
            };
        }

        /// <summary>
        /// Validates that the specified user belongs to an existing, active, KYC-verified,
        /// and non–self-excluded player account.
        /// </summary>
        /// <param name="userId">The ID of the user whose player profile is being validated.</param>
        /// <returns>
        /// The corresponding <see cref="Player"/> entity if validation succeeds.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when no player exists for the given user ID.
        /// </exception>
        /// <exception cref="EntityForbiddenException">
        /// Thrown when the user account is inactive, not KYC-verified, or the player is self-excluded.
        /// </exception>
        private async Task<Player> ValidatePlayer(Guid userId)
        {
            var result = await _playerAccessValidator.ValidateAsync(userId);

            if (!result.IsAllowed)
            {
                throw result.Failure switch
                {
                    PlayerAccessFailure.PlayerNotFound =>
                        new EntityNotFoundException(nameof(Player), "Player not found."),

                    PlayerAccessFailure.AccountInactive =>
                        new EntityForbiddenException(nameof(User), "Account is inactive."),

                    PlayerAccessFailure.KycPending =>
                        new EntityForbiddenException(nameof(Player), "KYC verification required."),

                    PlayerAccessFailure.SelfExcluded =>
                        new EntityForbiddenException(nameof(Player), "Self-excluded players cannot play or perform session actions."),

                    _ =>
                        new EntityForbiddenException(nameof(Player), "Access denied.")
                };
            }

            return result.Player!;
        }

        /// <summary>
        /// Retrieves and validates the HighLow game configuration, ensuring that the game exists
        /// and is currently enabled for play.
        /// </summary>
        /// <returns>
        /// The <see cref="Game"/> entity representing the HighLow game.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the HighLow game definition cannot be found.
        /// </exception>
        /// <exception cref="EntityForbiddenException">
        /// Thrown when the HighLow game exists but is disabled.
        /// </exception>
        private async Task<Game> ValidateGame()
        {
            var game = await _unitOfWork.GameRepository.GetByCodeAsync("highlow")
                ?? throw new EntityNotFoundException(nameof(Game), "HighLow game not found.");

            if (!game.IsEnabled)
                throw new EntityForbiddenException(nameof(Game), "HighLow game is disabled.");

            return game;
        }

        /// <summary>
        /// Validates that a HighLow game session exists, belongs to the specified user,
        /// and is currently active.
        /// </summary>
        /// <param name="sessionId">The external session ID returned by the HighLow Game API.</param>
        /// <param name="userId">The ID of the user attempting to access the session.</param>
        /// <returns>
        /// The <see cref="GameSession"/> entity if the session passes all validation rules.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when no session exists for the provided session ID.
        /// </exception>
        /// <exception cref="EntityForbiddenException">
        /// Thrown when the session does not belong to the user or is not active.
        /// </exception>
        private async Task<GameSession> ValidateSession(Guid sessionId, Guid userId)
        {
            var session = await _unitOfWork.GameSessionRepository.GetByExternalSessionIdAsync(sessionId)
                ?? throw new EntityNotFoundException(nameof(GameSession), "Game session not found.");

            if (session.UserId != userId)
                throw new EntityForbiddenException(nameof(GameSession), "Session does not belong to this user.");

            if (session.GameStatus != GameStatus.Active)
                throw new EntityForbiddenException(nameof(GameSession), "Session is not active.");

            return session;
        }
    }
}

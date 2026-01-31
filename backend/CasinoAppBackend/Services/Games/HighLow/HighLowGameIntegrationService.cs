using CasinoAppBackend.DTO.Games.HighLow;
using CasinoAppBackend.Exceptions;

namespace CasinoAppBackend.Services.Games.HighLow
{
    /// <summary>
    /// Provides integration functionality for interacting with the external HighLow Game API.
    /// </summary>
    public class HighLowGameIntegrationService : IHighLowGameIntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HighLowGameIntegrationService> _logger;

        public HighLowGameIntegrationService(HttpClient httpClient, ILogger<HighLowGameIntegrationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Starts a new HighLow game session for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user initiating the game session.</param>
        /// <returns>
        /// A <see cref="StartGameResponseDTO"/> containing session details returned by the HighLow Game API.
        /// </returns>
        /// <exception cref="ExternalServiceFailedException">
        /// Thrown when the HighLow Game API responds with a non-success status code.
        /// </exception>
        /// <exception cref="ExternalServiceInvalidResponseException">
        /// Thrown when the API returns an empty, null, or invalid session response.
        /// </exception>
        public async Task<StartGameResponseDTO> StartGameAsync(Guid userId)
        {
            _logger.LogInformation("Requesting HighLow game session start for UserId: {UserId}", userId);

            var response = await _httpClient.PostAsJsonAsync("api/highlowgame/start",userId);

            _ = response.IsSuccessStatusCode ? true
                : throw new ExternalServiceFailedException("GameStart",
                "HighLowGameApi returned error on start. Status: " + response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<StartGameResponseDTO>()
                ?? throw new ExternalServiceInvalidResponseException("GameStart",
                "HighLowGameApi returned empty or invalid StartGame response.");

            _logger.LogInformation("HighLow game session started with SessionId: {SessionId}.", result.SessionId);
            return result;
        }

        /// <summary>
        /// Starts a new round for an active HighLow game session.
        /// </summary>
        /// <param name="request">The round request payload containing session and round parameters.</param>
        /// <returns>
        /// A <see cref="StartRoundResultResponseDTO"/> containing the round result returned by the API.
        /// </returns>
        /// <exception cref="ExternalServiceFailedException">
        /// Thrown when the HighLow Game API responds with a non-success status code.
        /// </exception>
        /// <exception cref="ExternalServiceInvalidResponseException">
        /// Thrown when the API returns an empty, null, or invalid round result.
        /// </exception>
        public async Task<StartRoundResultResponseDTO> StartRoundAsync(StartRoundRequestDTO request)
        {
            _logger.LogInformation("Requesting HighLow round start for SessionId: {SessionId}.", request.SessionId);

            var response = await _httpClient.PostAsJsonAsync("api/highlowgame/round/start", request);

            _ = response.IsSuccessStatusCode ? true
                : throw new ExternalServiceFailedException("GameRound", 
                "HighLowGameApi returned error on round start. Status: " + response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<StartRoundResultResponseDTO>()
                ?? throw new ExternalServiceInvalidResponseException("GameRound", 
                "HighLowGameApi returned empty or invalid RoundResult response.");

            _logger.LogInformation("HighLow round result received for SessionId: {SessionId} and RoundNumber: {RoundNumber}.",
                request.SessionId, result.RoundNumber);
            return result;
        }

        /// <summary>
        /// Sends a request to the external HighLow Game API to acknowledge the end of an active session. 
        /// The external service confirms that the session ID was received and processed.
        /// </summary>
        /// <param name="userId">The ID of the user associated with the session.</param>
        /// <param name="sessionId">The ID of the session to end.</param>
        /// <returns>
        /// An <see cref="EndSessionResponseDTO"/> containing the external service's 
        /// acknowledgement (the session ID).
        /// </returns>
        /// <exception cref="ExternalServiceFailedException">
        /// Thrown if the HighLow Game API responds with a non-success HTTP status code.
        /// </exception>
        /// <exception cref="ExternalServiceInvalidResponseException">
        /// Thrown if the external API returns an empty or invalid acknowledgment.
        /// </exception>
        public async Task<EndSessionResponseDTO> EndSessionAsync(Guid userId, Guid sessionId)
        {
            _logger.LogInformation("Requesting HighLow session end with SessionId: {SessionId} for UserId: {UserId}",
                sessionId, userId);

            var request = new EndSessionRequestDTO
            {
                SessionId = sessionId
            };

            var response = await _httpClient.PostAsJsonAsync("api/highlowgame/session/end", request);

            _ = response.IsSuccessStatusCode ? true
                : throw new ExternalServiceFailedException("GameEndSession", 
                "HighLowGameApi returned error on session end. Status: " + response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<EndSessionResponseDTO>()
                ?? throw new ExternalServiceInvalidResponseException("GameEndSession", 
                "HighLowGameApi returned empty or invalid EndSession response.");

            _logger.LogInformation("HighLow session ended successfully for SessionId: {SessionId}",
                result.SessionId);
            return result;
        }

        /// <summary>
        /// Sends a request to the external HighLow Game API to acknowledge the time out of an active session. 
        /// The external service confirms that the session ID was received and processed.
        /// </summary>
        /// <param name="userId">The ID of the user associated with the session.</param>
        /// <param name="sessionId">The ID of the session being timed out.</param>
        /// <returns>
        /// A <see cref="EndSessionResponseDTO"/> containing the external service's 
        /// acknowledgement (the session ID).
        /// </returns>
        /// <exception cref="ExternalServiceFailedException">
        /// Thrown when the HighLow Game API responds with a non-success status code.
        /// </exception>
        /// <exception cref="ExternalServiceInvalidResponseException">
        /// Thrown if the external API returns an empty or invalid acknowledgment.
        /// </exception>
        public async Task<EndSessionResponseDTO> TimeoutSessionAsync(Guid userId, Guid sessionId)
        {
            _logger.LogInformation("Requesting HighLow session timeout for UserId: {UserId}, SessionId: {SessionId}",
                userId, sessionId);

            var request = new EndSessionRequestDTO
            {
                SessionId = sessionId
            };

            var response = await _httpClient.PostAsJsonAsync("api/highlowgame/session/timeout", request);

            _ = response.IsSuccessStatusCode ? true
                : throw new ExternalServiceFailedException("GameTimeoutSession", 
                "HighLowGameApi returned error on timeout session. Status: " + response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<EndSessionResponseDTO>()
                ?? throw new ExternalServiceInvalidResponseException("GameTimeoutSession", 
                "HighLowGameApi returned empty or invalid TimeoutSession response.");

            _logger.LogInformation(
                "HighLow session timed out successfully for SessionId: {SessionId} with Status: {GameStatus}",
                result.SessionId, result.GameStatus);
            return result;
        }
    }
}

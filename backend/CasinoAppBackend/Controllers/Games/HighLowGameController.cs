using CasinoAppBackend.DTO.Games.HighLow;
using CasinoAppBackend.Extensions;
using CasinoAppBackend.Services.Games.HighLow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Controllers.Games
{
    [Route("api/games/highlow")]
    [Authorize(Roles = "Player")]
    public class HighLowGameController : BaseController
    {
        private readonly IHighLowGameService _highLowGameService;

        public HighLowGameController(IHighLowGameService highLowGameService)
        {
            _highLowGameService = highLowGameService;
        }

        /// <summary>
        /// Starts a new HighLow game session for the authenticated player.
        /// </summary>
        /// <returns>
        /// A <see cref="StartGameResponseDTO"/> containing the external session ID 
        /// and the first drawn card (value, suit) from the external game API.
        /// </returns>
        /// <response code="200">Game session started successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">Player is inactive, self-excluded, or KYC not verified.</response>
        /// <response code="404">HighLow game configuration not found.</response>
        /// <response code="502">External HighLow API failed or returned invalid response.</response>
        [HttpPost("start")]
        public async Task<IActionResult> StartGame()
        {
            var userId = User.GetUserId();
            var result = await _highLowGameService.StartGameAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Plays a round in an active HighLow session for the authenticated player.
        /// </summary>
        /// <param name="request">Round input including bet amount, guess, and session ID.</param>
        /// <returns>
        /// A <see cref="StartRoundResultResponseDTO"/> containing the round outcome,
        /// the previous and newly drawn card values and suits, the win amount (if any),
        /// and the round number.
        /// </returns>
        /// <response code="200">Round processed successfully.</response>
        /// <response code="400">Invalid bet amount or other validation error.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">KYC missing, insufficient balance, or player restricted.</response>
        /// <response code="404">Game session not found.</response>
        /// <response code="502">External HighLow API failed or returned invalid response.</response>
        [HttpPost("round/start")]
        public async Task<IActionResult> StartRound([FromBody] StartRoundRequestDTO request)
        {
            var userId = User.GetUserId();
            var result = await _highLowGameService.StartRoundAsync(userId, request);
            return Ok(result);
        }

        /// <summary>
        /// Ends an active HighLow game session for the authenticated player.
        /// </summary>
        /// <param name="request">The session ID to end.</param>
        /// <returns>
        /// An <see cref="EndSessionResponseDTO"/> containing the session ID and the final session status.
        /// </returns>
        /// <response code="200">Session ended successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">Player is inactive or not allowed to end the session.</response>
        /// <response code="404">Game session not found.</response>
        /// <response code="502">External HighLow API failure.</response>
        [HttpPost("session/end")]
        public async Task<IActionResult> EndSession([FromBody] EndSessionRequestDTO request)
        {
            var userId = User.GetUserId();
            var result = await _highLowGameService.EndSessionAsync(userId, request.SessionId);
            return Ok(result);
        }

        /// <summary>
        /// Marks an active HighLow session as timed out.
        /// </summary>
        /// <param name="request">The session ID to time out.</param>
        /// <returns>
        /// An <see cref="EndSessionResponseDTO"/> containing the session ID and the updated timeout status.
        /// </returns>
        /// <response code="200">Session timed out successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">Player is not allowed to modify the session.</response>
        /// <response code="404">Game session not found.</response>
        /// <response code="502">External HighLow API failure.</response>
        [HttpPost("session/timeout")]
        public async Task<IActionResult> TimeoutSession([FromBody] EndSessionRequestDTO request)
        {
            var userId = User.GetUserId();
            var result = await _highLowGameService.TimeoutSessionAsync(userId, request.SessionId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the HighLow game configuration for the authenticated player.
        /// </summary>
        /// <returns>
        /// An <see cref="ConfigResponseDTO"/> containing the minimum and maximum bet limits,
        /// along with the player's current balance.
        /// </returns>
        /// <response code="200">Configuration returned successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            var userId = User.GetUserId();

            var config = await _highLowGameService.GetConfigAsync(userId);

            return Ok(config);
        }
    }
}
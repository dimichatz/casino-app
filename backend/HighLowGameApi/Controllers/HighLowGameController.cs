using HighLowGameApi.DTO;
using HighLowGameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HighLowGameApi.Controllers
{
    [Route("api/highlowgame")]
    [ApiController]
    public class HighLowGameController : ControllerBase
    {
        private readonly IHighLowGameService _highLowGameService;

        public HighLowGameController(IHighLowGameService highLowGameService)
        {
            _highLowGameService = highLowGameService;
        }

        /// <summary>
        /// Starts a new HighLow game session for the given user.
        /// </summary>
        /// <param name="userId">The ID of the user starting the session.</param>
        /// <returns>
        /// A <see cref="StartSessionResultDTO"/> containing the session ID
        /// and the initial card(vaue, suit) drawn for the game.
        /// </returns>
        /// <response code="200">Session created successfully.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] Guid userId)
        {
            var session = await _highLowGameService.StartSessionAsync(userId);
            return Ok(new StartSessionResultDTO
            {
                SessionId = session.Id,
                StartCardValue = session.StartCardValue,
                StartCardSuit = session.StartCardSuit
            });
        }

        /// <summary>
        /// Plays a new round within an existing HighLow game session.
        /// </summary>
        /// <param name="request">
        /// The round request containing session ID, bet amount, and the player's guess.
        /// </param>
        /// <returns>
        /// A <see cref="RoundResultDTO"/> containing the full outcome of the played round, 
        /// including whether the player won, the win amount, the previous and new card values 
        /// and suits, and the round number.
        /// </returns>
        /// <response code="200">Round completed successfully.</response>
        /// <response code="403">The session exists but is not active.</response>
        /// <response code="404">The session does not exist.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("round/start")]
        public async Task<IActionResult> StartRound([FromBody] StartRoundRequestDTO request)
        {
            var round = await _highLowGameService.StartRoundAsync(request.SessionId, request.BetAmount, request.GuessType);

            return Ok(new RoundResultDTO
            {
                IsWin = round.IsWin,
                WinAmount = round.WinAmount,
                PreviousCardValue = round.PreviousCardValue,
                NewCardValue = round.NewCardValue,
                PreviousCardSuit = round.PreviousCardSuit,
                NewCardSuit = round.NewCardSuit,
                RoundNumber = round.RoundNumber
            });
        }

        /// <summary>
        /// Ends an existing HighLow game session.
        /// </summary>
        /// <param name="request">The request containing the session ID to end.</param>
        /// <returns>
        /// A response object containing the session ID and final session status.
        /// </returns>
        /// <response code="200">Round completed successfully.</response>
        /// <response code="403">The session exists but is not active.</response>
        /// <response code="404">The session does not exist.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("session/end")]
        public async Task<IActionResult> EndSession([FromBody] EndSessionRequestDTO request)
        {
            var session = await _highLowGameService.EndSessionAsync(request.SessionId);

            return Ok(new
            {
                SessionId = session.Id,
                Status = session.GameStatus.ToString()
            });
        }

        /// <summary>
        /// Marks an active HighLow session as timed out due to inactivity.
        /// </summary>
        /// <param name="request">The request containing the session ID to time out.</param>
        /// <returns>
        /// A response object containing the session ID and updated session status.
        /// </returns>
        /// <response code="200">Round completed successfully.</response>
        /// <response code="403">The session exists but is not active.</response>
        /// <response code="404">The session does not exist.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("session/timeout")]
        public async Task<IActionResult> TimeoutSession([FromBody] EndSessionRequestDTO request)
        {
            var session = await _highLowGameService.TimeoutSessionAsync(request.SessionId);

            return Ok(new
            {
                SessionId = session.Id,
                Status = session.GameStatus.ToString()
            });
        }
    }
}

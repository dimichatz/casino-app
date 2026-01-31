using HighLowGameApi.Core.Enums;
using HighLowGameApi.Data;
using HighLowGameApi.Exceptions;
using HighLowGameApi.Helpers;
using HighLowGameApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HighLowGameApi.Services
{
    public class HighLowGameService : IHighLowGameService
    {
        private readonly ILogger<HighLowGameService> _logger;
        private readonly IGameSessionRepository _sessionRepository;
        private readonly IGameRoundRepository _roundRepository;
        private readonly HighLowGameApiDbContext _context;
        private readonly ICardGenerator _cardGenerator;

        public HighLowGameService(ILogger<HighLowGameService> logger,
            IGameSessionRepository sessionRepository, IGameRoundRepository roundRepository, 
            HighLowGameApiDbContext context, ICardGenerator cardGenerator)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
            _roundRepository = roundRepository;
            _context = context;
            _cardGenerator = cardGenerator;
        }

        /// <summary>
        /// Starts a new HighLow game session for the specified user.
        /// Generates an initial random card and initializes the game state.
        /// </summary>
        /// <param name="userId">The ID of the user starting the game session.</param>
        /// <returns>
        /// A <see cref="GameSession"/> representing the newly created HighLow game session.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when the session cannot be created or saved.
        /// </exception>
        public async Task<GameSession> StartSessionAsync(Guid userId)
        {
            var (Value, Suit) = _cardGenerator.DrawRandomCard();

            var session = new GameSession
            {
                UserId = userId,
                StartCardValue = Value,
                StartCardSuit = Suit,
                CurrentCardValue = Value,
                CurrentCardSuit = Suit,
                GameStatus = GameStatus.Active
            };

            await _sessionRepository.AddAsync(session);
            await _context.SaveChangesAsync();
            _logger.LogInformation("HighLow Session Started for UserId: {UserId} with SessionId: {SessionId}",
                userId, session.Id);
            return session;
        }

        /// <summary>
        /// Plays a new round in an active HighLow game session.
        /// Generates a new card, determines whether the player's guess is correct,
        /// calculates winnings, and updates the session state.
        /// </summary>
        /// <param name="sessionId">The ID of the active game session.</param>
        /// <param name="betAmount">The amount the player bet on the round.</param>
        /// <param name="guess">The player's guess (Higher, Lower, or Equal).</param>
        /// <returns>
        /// A <see cref="GameRound"/> containing the outcome of the played round,
        /// including whether the player won and the payout amount.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the session does not exist.
        /// </exception>
        /// <exception cref="SessionNotActiveException">
        /// Thrown when the session is not in an active state.
        /// </exception>
        public async Task<GameRound> StartRoundAsync(Guid sessionId, decimal betAmount, GuessType guess)
        {
            var session = await _sessionRepository.GetAsync(sessionId)
                ?? throw new EntityNotFoundException("Session not found.");

            if (session.GameStatus != GameStatus.Active)
                throw new SessionNotActiveException("Session is not active.");

            int previousRoundCount = await _context.GameRounds
                .Where(r => r.GameSessionId == sessionId)
                .CountAsync();

            var (Value, Suit) = _cardGenerator.DrawRandomCard();

            int newRank = GetRank(Value);
            int currentRank = GetRank(session.CurrentCardValue);

            bool isWin = guess switch
            {
                GuessType.Higher => newRank > currentRank,
                GuessType.Lower => newRank < currentRank,
                GuessType.Equal => newRank == currentRank,
                _ => false
            };

            decimal winAmount = 0;

            if (isWin)
            {
                winAmount = guess switch
                {
                    GuessType.Equal => betAmount * 10,
                    GuessType.Higher => betAmount * 2,
                    GuessType.Lower => betAmount * 2,
                    _ => 0
                };
            }

            var round = new GameRound
            {
                RoundNumber = previousRoundCount + 1,
                PreviousCardValue = session.CurrentCardValue,
                PreviousCardSuit = session.CurrentCardSuit,
                GuessType = guess,
                BetAmount = betAmount,
                NewCardValue = Value,
                NewCardSuit = Suit,
                IsWin = isWin,
                WinAmount = winAmount,
                GameSessionId = session.Id
            };

            await _roundRepository.AddAsync(round);

            session.CurrentCardValue = Value;
            session.CurrentCardSuit = Suit;

            await _sessionRepository.UpdateAsync(session);
            await _context.SaveChangesAsync();
            _logger.LogInformation("HighLow Round Played with SessionId: {SessionId}, RoundNumber: {RoundNumber}," +
                " Bet: {Bet}, Guess: {Guess}, Result: {Result} and WinAmount: {WinAmount}",
                session.Id, round.RoundNumber, betAmount, guess, isWin ? "WIN" : "LOSE", winAmount);
            return round;
        }

        /// <summary>
        /// Ends an active HighLow game session by marking it as terminated.
        /// </summary>
        /// <param name="sessionId">The ID of the session to end.</param>
        /// <returns>
        /// The updated <see cref="GameSession"/> with its status set to <see cref="GameStatus.Terminated"/>.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the session does not exist.
        /// </exception>
        /// <exception cref="SessionNotActiveException">
        /// Thrown when the session is not in an active state.
        /// </exception>
        public async Task<GameSession> EndSessionAsync(Guid sessionId)
        {
            var session = await _sessionRepository.GetAsync(sessionId)
                ?? throw new EntityNotFoundException("Session not found.");

            if (session.GameStatus != GameStatus.Active)
                throw new SessionNotActiveException("Session is not active.");

            session.GameStatus = GameStatus.Terminated;

            await _sessionRepository.UpdateAsync(session);
            await _context.SaveChangesAsync();
            _logger.LogInformation("HighLow Session Ended with SessionId: {SessionId} and Status: Terminated",
                sessionId);
            return session;
        }

        /// <summary>
        /// Marks an active HighLow game session as timed out, typically due to inactivity.
        /// </summary>
        /// <param name="sessionId">The ID of the session to time out.</param>
        /// <returns>
        /// The updated <see cref="GameSession"/> with its status set to <see cref="GameStatus.Timeout"/>.
        /// </returns>
        /// <exception cref="EntityNotFoundException">
        /// Thrown when the session does not exist.
        /// </exception>
        /// <exception cref="SessionNotActiveException">
        /// Thrown when the session is not in an active state.
        /// </exception>
        public async Task<GameSession> TimeoutSessionAsync(Guid sessionId)
        {
            var session = await _sessionRepository.GetAsync(sessionId)
                ?? throw new EntityNotFoundException("Session not found.");

            if (session.GameStatus != GameStatus.Active)
                throw new SessionNotActiveException("Session is not active.");

            session.GameStatus = GameStatus.Timeout;

            await _sessionRepository.UpdateAsync(session);
            await _context.SaveChangesAsync();
            _logger.LogInformation("HighLow Session Ended with SessionId: {SessionId} and Status: Timeout",
                sessionId);
            return session;
        }

        private int GetRank(int value)
        {
            return value == 1 ? 14 : value;
        }
    }
}

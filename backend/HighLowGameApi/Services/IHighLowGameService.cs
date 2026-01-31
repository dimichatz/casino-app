using HighLowGameApi.Core.Enums;
using HighLowGameApi.Data;

namespace HighLowGameApi.Services
{
    public interface IHighLowGameService
    {
        Task<GameSession> StartSessionAsync(Guid userId);
        Task<GameRound> StartRoundAsync(Guid sessionId, decimal betAmount, GuessType guess);
        Task<GameSession> EndSessionAsync(Guid sessionId);
        Task<GameSession> TimeoutSessionAsync(Guid sessionId);
    }
}

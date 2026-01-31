using CasinoAppBackend.Data;

namespace CasinoAppBackend.Repositories
{
    public interface IGameSessionRepository
    {
        Task<GameSession?> GetByExternalSessionIdAsync(Guid externalSessionId);
    }
}

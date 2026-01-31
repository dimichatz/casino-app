using CasinoAppBackend.Data;

namespace CasinoAppBackend.Repositories
{
    public interface IPlayerLimitRepository
    {
        Task<PlayerLimit?> GetByPlayerIdAsync(Guid playerId);
        Task<List<PlayerLimit>> GetAllWithPendingLimitsAsync();
    }
}

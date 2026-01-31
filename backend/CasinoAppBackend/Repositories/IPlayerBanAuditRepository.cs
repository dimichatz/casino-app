using CasinoAppBackend.Data;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Repositories
{
    public interface IPlayerBanAuditRepository
    {
        Task<List<PlayerBanAudit>> GetAuditsByPlayerIdAsync(Guid playerId);
        Task<PaginatedResult<PlayerBanAudit>> GetPaginatedAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize);
    }
}

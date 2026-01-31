using CasinoAppBackend.Data;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Repositories
{
    public interface IPlayerDetailsAuditRepository
    {
        Task<List<PlayerDetailsAudit>> GetAuditsByPlayerIdAsync(Guid playerId);
        Task<PaginatedResult<PlayerDetailsAudit>> GetPaginatedAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize);
    }
}

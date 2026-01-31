using CasinoAppBackend.Data;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Repositories
{
    public interface IPlayerLimitAuditRepository 
    {
        Task<List<PlayerLimitAudit>> GetAuditsByPlayerIdAsync(Guid playerId);
        Task<PaginatedResult<PlayerLimitAudit>> GetPaginatedAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize);
    }
}

using CasinoAppBackend.Data;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Repositories
{
    public interface IPlayerSelfExclusionAuditRepository
    {
        Task<List<PlayerSelfExclusionAudit>> GetAuditsByPlayerIdAsync(Guid playerId);
        Task<PaginatedResult<PlayerSelfExclusionAudit>> GetPaginatedAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize);
    }
}

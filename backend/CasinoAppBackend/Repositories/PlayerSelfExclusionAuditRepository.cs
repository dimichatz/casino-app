using CasinoAppBackend.Data;
using CasinoAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class PlayerSelfExclusionAuditRepository : BaseRepository<PlayerSelfExclusionAudit>, IPlayerSelfExclusionAuditRepository
    {
        public PlayerSelfExclusionAuditRepository(CasinoAppBackendDbContext context) : base(context)
        {
        }

        public async Task<List<PlayerSelfExclusionAudit>> GetAuditsByPlayerIdAsync(Guid playerId)
        {
            return await context.PlayerSelfExclusionAudits
                .Where(a => a.PlayerId == playerId)
                .OrderByDescending(a => a.InsertedAt)
                .ToListAsync();
        }

        public async Task<PaginatedResult<PlayerSelfExclusionAudit>> GetPaginatedAuditsByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            IQueryable<PlayerSelfExclusionAudit> query = context.PlayerSelfExclusionAudits
                .Where(a => a.PlayerId == playerId);

            var totalRecords = await query.CountAsync();

            var audits = await query
                .OrderByDescending(a => a.InsertedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<PlayerSelfExclusionAudit>
            {
                Data = audits,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}

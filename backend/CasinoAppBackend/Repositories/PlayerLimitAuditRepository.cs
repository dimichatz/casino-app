using CasinoAppBackend.Data;
using CasinoAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class PlayerLimitAuditRepository : BaseRepository<PlayerLimitAudit>, IPlayerLimitAuditRepository
    {
        public PlayerLimitAuditRepository(CasinoAppBackendDbContext context) : base(context)
        {
        }

        public async Task<List<PlayerLimitAudit>> GetAuditsByPlayerIdAsync(Guid playerId)
        {
            return await context.PlayerLimitAudits
                .Where(a => a.PlayerId == playerId)
                .OrderByDescending(a => a.InsertedAt)
                .ToListAsync();
        }

        public async Task<PaginatedResult<PlayerLimitAudit>> GetPaginatedAuditsByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            IQueryable<PlayerLimitAudit> query = context.PlayerLimitAudits
                .Where(a => a.PlayerId == playerId);

            var totalRecords = await query.CountAsync();

            var audits = await query
                .OrderByDescending(a => a.InsertedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<PlayerLimitAudit>
            {
                Data = audits,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}

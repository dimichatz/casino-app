using CasinoAppBackend.Data;
using CasinoAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class PlayerDetailsAuditRepository : BaseRepository<PlayerDetailsAudit>, IPlayerDetailsAuditRepository
    {
        public PlayerDetailsAuditRepository(CasinoAppBackendDbContext context) : base(context)
        {
        }

        public async Task<List<PlayerDetailsAudit>> GetAuditsByPlayerIdAsync(Guid playerId)
        {
            return await context.PlayerDetailsAudits
                .Where(a => a.PlayerId == playerId)
                .OrderByDescending(a => a.InsertedAt)
                .ToListAsync();
        }

        public async Task<PaginatedResult<PlayerDetailsAudit>> GetPaginatedAuditsByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            IQueryable<PlayerDetailsAudit> query = context.PlayerDetailsAudits
                .Where(a => a.PlayerId == playerId);

            var totalRecords = await query.CountAsync();

            var audits = await query
                .OrderByDescending(a => a.InsertedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<PlayerDetailsAudit>
            {
                Data = audits,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}

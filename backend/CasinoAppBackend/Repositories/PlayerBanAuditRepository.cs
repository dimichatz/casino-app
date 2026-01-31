using CasinoAppBackend.Data;
using CasinoAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class PlayerBanAuditRepository : BaseRepository<PlayerBanAudit>, IPlayerBanAuditRepository
    {
        public PlayerBanAuditRepository(CasinoAppBackendDbContext context) : base(context)
        {
        }

        public async Task<List<PlayerBanAudit>> GetAuditsByPlayerIdAsync(Guid playerId)
        {
            return await context.PlayerBanAudits
             .Where(a => a.PlayerId == playerId)
             .OrderByDescending(a => a.InsertedAt)
             .ToListAsync();
        }

        public async Task<PaginatedResult<PlayerBanAudit>> GetPaginatedAuditsByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            IQueryable<PlayerBanAudit> query = context.PlayerBanAudits
                .Where(a => a.PlayerId == playerId);

            var totalRecords = await query.CountAsync();

            var audits = await query
                .OrderByDescending(a => a.InsertedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<PlayerBanAudit>
            {
                Data = audits,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}

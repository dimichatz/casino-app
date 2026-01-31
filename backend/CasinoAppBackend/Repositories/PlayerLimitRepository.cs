using CasinoAppBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class PlayerLimitRepository : BaseRepository<PlayerDetailsAudit>, IPlayerLimitRepository
    {
        public PlayerLimitRepository(CasinoAppBackendDbContext context) : base(context)
        {
        }

        public async Task<PlayerLimit?> GetByPlayerIdAsync(Guid playerId) =>
            await context.PlayerLimits.FirstOrDefaultAsync(k => k.PlayerId == playerId);

        public async Task<List<PlayerLimit>> GetAllWithPendingLimitsAsync()
        {
            return await context.PlayerLimits
                .Where(p =>
                       p.PendingDepositDailyLimit != null ||
                       p.PendingDepositWeeklyLimit != null ||
                       p.PendingDepositMonthlyLimit != null ||
                       p.PendingLossDailyLimit != null ||
                       p.PendingLossWeeklyLimit != null ||
                       p.PendingLossMonthlyLimit != null)
                .ToListAsync();
        }
    }
}

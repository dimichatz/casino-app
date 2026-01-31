using System.Linq.Expressions;
using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Data;
using CasinoAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(CasinoAppBackendDbContext context) 
            : base(context)
        {
        }

        public async Task<List<User>> GetAllUsersPlayersAsync()
        {
            var usersWithPlayerRole = await context.Users
                .Where(u => u.UserRole == UserRole.Player)
                .Include(u => u.Player)
                .OrderByDescending(u => u.InsertedAt)
                .ToListAsync();

            return usersWithPlayerRole;
        }

        public async Task<List<PlayerDetailsAudit>> GetAllPlayerDetailsAuditsByPlayerIdAsync(Guid playerId)
        {

            var auditsByPlayerId = await context.PlayerDetailsAudits
                .Where(a => a.PlayerId == playerId)
                .Include(a => a.Player)
                .OrderByDescending(a => a.InsertedAt)
                .ToListAsync();

            return auditsByPlayerId;
        }

        public async Task<PaginatedResult<PlayerDetailsAudit>> GetPaginatedPlayerDetailsAuditsByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            IQueryable<PlayerDetailsAudit> query = context.PlayerDetailsAudits
                .Where(a => a.PlayerId == playerId);

            var totalRecords = await query.CountAsync();

            var auditsByPlayerId = await query
                .OrderByDescending(p => p.InsertedAt)
                .Include(a => a.Player)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<PlayerDetailsAudit>
            {
                Data = auditsByPlayerId,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<bool> IsPlayerSelfExcludedAsync(Guid id)
        {
            var player = await context.Players.FirstOrDefaultAsync(p => p.Id == id);
            return player != null && player.IsSelfExcluded;
        }

        public async Task StartPlayerSelfExclusionAsync(Guid id, int numberOfDays)
        {
            var player = await context.Players.FirstOrDefaultAsync(p => p.Id == id);
            if (player is not null)
            {
                player.IsSelfExcluded = true;
                var start = DateTime.UtcNow;

                player.SelfExclusionStart = start;
                player.SelfExclusionEnd = start.AddDays((double)numberOfDays);
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<Player>> GetPlayersFilteredAsync(string? search, bool? isActive, KycStatus? kycStatus)
        {
            var predicates = new List<Expression<Func<Player, bool>>>();

            IQueryable<Player> query = context.Players
                .Include(p => p.User)
                .Include(p => p.KycDocument);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                predicates.Add(p =>
                    EF.Functions.Like(p.User.Username, pattern) ||
                    EF.Functions.Like(p.User.Email, pattern));
            }

            if (isActive.HasValue)
                predicates.Add(p => p.User.IsActive == isActive.Value);

            if (kycStatus.HasValue)
                predicates.Add(p => p.KycDocument != null && p.KycDocument.KycStatus == kycStatus);

            predicates.ForEach(predicate => query = query.Where(predicate));

            return await query
                .OrderByDescending(p => p.InsertedAt)
                .ToListAsync();
        }

        public async Task<PaginatedResult<Player>> GetPaginatedPlayersFilteredAsync(int pageNumber, int pageSize,
            string? search, bool? isActive, KycStatus? kycStatus)
        {
            int skip = (pageNumber - 1) * pageSize;
            var predicates = new List<Expression<Func<Player, bool>>>();

            IQueryable<Player> query = context.Players
                .Include(p => p.User)
                .Include(p => p.KycDocument);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                predicates.Add(p =>
                    EF.Functions.Like(p.User.Username, pattern) ||
                    EF.Functions.Like(p.User.Email, pattern));
            }

            if (isActive.HasValue)
                predicates.Add(p => p.User.IsActive == isActive.Value);

            if (kycStatus.HasValue)
                predicates.Add(p => p.KycDocument != null && p.KycDocument.KycStatus == kycStatus);

            predicates.ForEach(predicate => query = query.Where(predicate));

            var totalRecords = await query.CountAsync();

            var playersFiltered = await query
                .OrderByDescending(p => p.InsertedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Player>
            {
                Data = playersFiltered,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Player?> GetByUserIdAsync(Guid userId) =>
            await context.Players
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        public async Task<Player?> GetFullDetailsByUserIdAsync(Guid userId) =>
            await context.Players
            .Include(p => p.User)
            .Include(p => p.KycDocument)
            .Include(p => p.PlayerLimit)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        public async Task<Player?> GetFullDetailsByIdAsync(Guid id) =>
            await context.Players
            .Include(p => p.User)
            .Include(p => p.KycDocument)
            .Include(p => p.PlayerLimit)
            .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Player?> GetByAccountIdAsync(Guid accountId) =>
            await context.Players
            .Include(p => p.Account)
            .FirstOrDefaultAsync(p => p.Account.Id == accountId);

        public async Task<List<Player>> GetAllPlayersWithExpiredSelfExclusionAsync(DateTime utcNow)
        {
            return await context.Players
                .Where(p => p.IsSelfExcluded
                            && p.SelfExclusionEnd != null
                            && p.SelfExclusionEnd <= utcNow
                            && p.SelfExclusionPeriod != SelfExclusionPeriod.Permanent)
                .Include(p => p.User)
                .ToListAsync();
        }
    }
}

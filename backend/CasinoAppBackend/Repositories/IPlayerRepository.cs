using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Data;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Repositories
{
    public interface IPlayerRepository
    {
        Task<List<User>> GetAllUsersPlayersAsync();
        Task<List<PlayerDetailsAudit>> GetAllPlayerDetailsAuditsByPlayerIdAsync(Guid playerId);
        Task<PaginatedResult<PlayerDetailsAudit>> GetPaginatedPlayerDetailsAuditsByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize);
        Task<bool> IsPlayerSelfExcludedAsync(Guid id);
        Task StartPlayerSelfExclusionAsync(Guid id, int breakDays);
        Task<List<Player>> GetPlayersFilteredAsync(string? search, bool? isActive, KycStatus? kycStatus);
        Task<PaginatedResult<Player>> GetPaginatedPlayersFilteredAsync(int pageNumber, int pageSize,
            string? search, bool? isActive, KycStatus? kycStatus);
        Task<Player?> GetByUserIdAsync(Guid userId);
        Task<Player?> GetFullDetailsByUserIdAsync(Guid userId);
        Task<Player?> GetFullDetailsByIdAsync(Guid id);
        Task<Player?> GetByAccountIdAsync(Guid accountId);
        Task<List<Player>> GetAllPlayersWithExpiredSelfExclusionAsync(DateTime utcNow);
    }
}
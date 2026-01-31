using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Core.Filters;
using CasinoAppBackend.DTO;
using CasinoAppBackend.DTO.AuditReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerUpdateFullDetailsAdminDTO;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Services
{
    public interface IAdminService
    {
        Task<AdminReadOnlyDTO> CreateAdminAsync(AdminCreateDTO request);
        Task UpdateAdminAsync(Guid id, AdminUpdateDTO request);
        Task ChangePasswordAsync(Guid id, UserChangePasswordDTO request);
        Task<AdminReadOnlyDTO> GetAdminByIdAsync(Guid id);
        Task<List<AdminReadOnlyDTO>> GetAdminsFilteredAsync(string? search,
            bool? isActive);
        Task<PaginatedResult<AdminReadOnlyDTO>> GetPaginatedAdminsFilteredAsync(int pageNumber, int pageSize,
            string? search, bool? isActive);

        Task<AppSettingsReadOnlyDTO> GetAppSettingsAsync();
        Task UpdateAppSettingsAsync(AppSettingsUpdateDTO request);
        Task ChangeAdminStatusAsync(Guid id, bool isActive);

        Task UpdatePlayerAsync(Guid playerId, PlayerUpdateFullDetailsAdminDTO request, string username);
        Task<PlayerFullDetailsReadOnlyDTO> GetPlayerByIdAsync(Guid id);
        Task<List<PlayerReadOnlyDTO>> GetPlayersFilteredAsync(string? search,
            bool? isActive, KycStatus? kycStatus);
        Task<PaginatedResult<PlayerReadOnlyDTO>> GetPaginatedPlayersFilteredAsync(int pageNumber, int pageSize,
            string? search, bool? isActive, KycStatus? kycStatus); 
        Task<List<PlayerSelfExclusionAuditReadOnlyDTO>> GetPlayerSelfExclusionAuditsByPlayerIdAsync(Guid playerId);
        Task<List<PlayerLimitAuditReadOnlyDTO>> GetPlayerLimitAuditsByPlayerIdAsync(Guid playerId);
        Task<List<PlayerDetailsAuditReadOnlyDTO>> GetPlayerDetailsAuditsByPlayerIdAsync(Guid playerId);
        Task<List<PlayerBanAuditReadOnlyDTO>> GetPlayerBanAuditsByPlayerIdAsync(Guid playerId);
        Task<PaginatedResult<PlayerSelfExclusionAuditReadOnlyDTO>> GetPaginatedPlayerSelfExclusionAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize);
        Task<PaginatedResult<PlayerLimitAuditReadOnlyDTO>> GetPaginatedPlayerLimitAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize);
        Task<PaginatedResult<PlayerDetailsAuditReadOnlyDTO>> GetPaginatedPlayerDetailsAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize);
        Task<PaginatedResult<PlayerBanAuditReadOnlyDTO>> GetPaginatedPlayerBanAuditsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize);
        Task<List<TransactionReadOnlyDTO>> GetPlayerTransactionsByPlayerIdAsync(Guid playerId, TransactionFilterDTO filters);
        Task<PaginatedResult<TransactionReadOnlyDTO>> GetPaginatedPlayerTransactionsByPlayerIdAsync(Guid playerId,
            int pageNumber, int pageSize, TransactionFilterDTO filters);
        Task<AttachmentReadOnlyDTO> GetPlayerKycAttachmentAsync(Guid playerId);

        Task<PaginatedResult<GameReadOnlyDTO>> GetPaginatedGamesFilteredAsync(int pageNumber, int pageSize,
            string? search);
        Task<GameReadOnlyDTO> GetGameByIdAsync(Guid id);
        Task ChangeGameStatusAsync(Guid id, bool isEnabled);
    }
}

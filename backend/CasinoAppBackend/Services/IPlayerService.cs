using CasinoAppBackend.Core.Filters;
using CasinoAppBackend.DTO;
using CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO;
using CasinoAppBackend.DTO.PlayerUpdateFullDetailsPlayerDTO;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Services
{
    public interface IPlayerService
    {
        Task<PlayerReadOnlyDTO> SignUpPlayerAsync(PlayerSignUpDTO request);
        Task UpdatePlayerAsync(Guid userId, PlayerUpdateFullDetailsPlayerDTO request);
        Task ChangePasswordAsync(Guid userId, UserChangePasswordDTO request);
        Task<PlayerFullDetailsReadOnlyDTO> GetPlayerByUserIdAsync(Guid userId);
        Task<PlayerFullDetailsReadOnlyDTO> GetPlayerFullDetailsByUserIdAsync(Guid userId);
        Task<List<TransactionReadOnlyDTO>> GetTransactionsByUserIdAsync(Guid userId, TransactionFilterDTO filters);
        Task<PaginatedResult<TransactionReadOnlyDTO>> GetPaginatedTransactionsByUserIdAsync(Guid userId,
            int pageNumber, int pageSize, TransactionFilterDTO filters);
        Task UploadKycAttachmentAsync(Guid userId, IFormFile file);
    }
}

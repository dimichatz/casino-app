using CasinoAppBackend.Core.Filters;
using CasinoAppBackend.DTO;
using CasinoAppBackend.Models;
using Microsoft.Identity.Client;

namespace CasinoAppBackend.Services
{
    public interface IAccountService
    {
        Task<TransactionReadOnlyDTO> ProcessTransactionAsync(Guid playerId, TransactionRequestDTO request);
        Task<List<TransactionReadOnlyDTO>> GetAllTransactionsByAccountIdFilteredAsync(Guid accountId,
            TransactionFilterDTO transactionFilterDTO);
        Task<PaginatedResult<TransactionReadOnlyDTO>> GetPaginatedTransactionsByAccountIdFilteredAsync(Guid accountId, int pageNumber, int pageSize,
            TransactionFilterDTO transactionFilterDTO);
        Task<PlayerBalanceReadOnlyDTO> GetPlayerBalanceAsync(Guid playerId);
    }
}

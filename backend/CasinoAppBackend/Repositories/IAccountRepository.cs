using System.Linq.Expressions;
using CasinoAppBackend.Data;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByPlayerIdAsync(Guid playerId);
        Task<List<Transaction>> GetAllTransactionsByAccountIdFilteredAsync(Guid accountId,
            List<Expression<Func<Transaction, bool>>>? predicates = null);
        Task<PaginatedResult<Transaction>> GetPaginatedTransactionsByAccountIdFilteredAsync(Guid accountId, int pageNumber, int pageSize,
            List<Expression<Func<Transaction, bool>>>? predicates = null);
    }
}

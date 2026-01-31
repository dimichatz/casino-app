using System.Linq.Expressions;
using CasinoAppBackend.Data;
using CasinoAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(CasinoAppBackendDbContext context) 
            : base(context)
        {
        }

        public async Task<Account?> GetByPlayerIdAsync(Guid playerId) =>
            await context.Accounts.FirstOrDefaultAsync(a => a.PlayerId == playerId);

        public async Task<List<Transaction>> GetAllTransactionsByAccountIdFilteredAsync(Guid accountId,
            List<Expression<Func<Transaction, bool>>>? predicates = null)
        {
            IQueryable<Transaction> query = context.Transactions
                .Where(t => t.AccountId == accountId);

            predicates?.ForEach(predicate => query = query.Where(predicate));

            var transactionsByAccountId = await query
                .Include(t => t.Account)
                .OrderByDescending(t => t.InsertedAt)
                .ToListAsync();

            return transactionsByAccountId;
        }

        public async Task<PaginatedResult<Transaction>> GetPaginatedTransactionsByAccountIdFilteredAsync(Guid accountId, int pageNumber, int pageSize,
            List<Expression<Func<Transaction, bool>>>? predicates = null)
        {
            int skip = (pageNumber - 1) * pageSize;

            IQueryable<Transaction> query = context.Transactions
                .Where(t => t.AccountId == accountId);

            predicates?.ForEach(predicate => query = query.Where(predicate));

            var totalRecords = await query.CountAsync();

            var transactionsByAccountId = await query
                .Include(t => t.Account)
                .OrderByDescending(a => a.InsertedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Transaction>
            {
                Data = transactionsByAccountId,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(CasinoAppBackendDbContext context) 
            : base(context)
        {
        }

        public async Task<long> GenerateTransactionNumberAsync()
        {
            try
            {
                var nextValue = await context.Database
                    .SqlQueryRaw<long>("SELECT NEXT VALUE FOR TransactionNumberSequence")
                    .FirstAsync();

                return nextValue;
            }
            catch
            {
                // Fallback for local/test DBs without sequences
                var last = await context.Transactions
                    .OrderByDescending(t => t.TransactionNumber)
                    .Select(t => t.TransactionNumber)
                    .FirstOrDefaultAsync();

                return last == 0 ? 100000 : last + 1;
            }
        }

        public async Task<decimal> GetTotalTransactionAmountByTypeAsync(Guid accountId, TransactionType transactionType, DateTimeOffset startDate)
        {
            return await context.Transactions
                .Where(t => t.AccountId == accountId
                && t.TransactionType == transactionType
                && t.TransactionStatus == TransactionStatus.Completed
                && t.InsertedAt >= startDate && t.InsertedAt <= DateTimeOffset.UtcNow)
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;
        }

        public async Task<Transaction?> GetByGameRoundIdAsync(int gameRoundId) =>
            await context.Transactions.FirstOrDefaultAsync(t => t.GameRoundId == gameRoundId);
    }
}

using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Data;

namespace CasinoAppBackend.Repositories
{
    public interface ITransactionRepository
    {
        Task<long> GenerateTransactionNumberAsync();
        Task<decimal> GetTotalTransactionAmountByTypeAsync(Guid accountId, TransactionType transactionType, DateTimeOffset startDate);
        Task<Transaction?> GetByGameRoundIdAsync(int gameRoundId);
    }
}

using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO
{
    public class TransactionReadOnlyDTO
    {
        public Guid Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public long TransactionNumber { get; set; }
        public string? GameId { get; set; }
        public string? GameName { get; set; }
        public string? GameRoundId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public decimal? OldBalance { get; set; }
        public decimal? NewBalance { get; set; }
        public DateTimeOffset InsertedAt { get; set; }
    }
}

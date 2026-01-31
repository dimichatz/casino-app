using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.Data
{
    public class Transaction : CreatableEntity
    {
        public TransactionType TransactionType { get; set; }
        public Guid? GameId { get; set; }
        public int? GameRoundId { get; set; }
        public string? GameName { get; set; }
        public long TransactionNumber { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public TransactionStatus TransactionStatus { get; set; }
        public decimal? OldBalance { get; set; }
        public decimal? NewBalance { get; set; }
        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Game? Game { get; set; } = null!;
    }
}

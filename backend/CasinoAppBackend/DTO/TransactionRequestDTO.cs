using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO
{
    public class TransactionRequestDTO
    {
        public TransactionType? TransactionType { get; set; }
        public Guid? GameId { get; set; }
        public int? GameRoundId { get; set; }
        public decimal Amount { get; set; }
        public decimal? BetAmount { get; set; }
        public string? Currency { get; set; }
    }
}

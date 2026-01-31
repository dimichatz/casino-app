using CasinoAppBackend.Core.Enums.Games.LowHigh;

namespace CasinoAppBackend.DTO.Games.HighLow
{
    public class StartRoundRequestDTO
    {
        public Guid SessionId { get; set; }
        public decimal BetAmount { get; set; }
        public GuessType GuessType { get; set; }
    }
}

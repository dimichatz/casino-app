using HighLowGameApi.Core.Enums;

namespace HighLowGameApi.DTO
{
    public class StartRoundRequestDTO
    {
        public Guid SessionId { get; set; }
        public decimal BetAmount { get; set; }
        public GuessType GuessType { get; set; }
    }
}

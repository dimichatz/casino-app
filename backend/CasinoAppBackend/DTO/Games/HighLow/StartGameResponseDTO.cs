using CasinoAppBackend.Core.Enums.Games.LowHigh;

namespace CasinoAppBackend.DTO.Games.HighLow
{
    public class StartGameResponseDTO
    {
        public Guid SessionId { get; set; }
        public int StartCardValue { get; set; }
        public SuitType StartCardSuit { get; set; }
    }
}

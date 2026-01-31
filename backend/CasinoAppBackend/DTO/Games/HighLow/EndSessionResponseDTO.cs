using CasinoAppBackend.Core.Enums.Games.LowHigh;

namespace CasinoAppBackend.DTO.Games.HighLow
{
    public class EndSessionResponseDTO
    {
        public Guid SessionId { get; set; }
        public GameStatus GameStatus { get; set; }
    }
}

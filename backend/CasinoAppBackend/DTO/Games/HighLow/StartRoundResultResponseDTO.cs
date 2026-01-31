using CasinoAppBackend.Core.Enums.Games.LowHigh;

namespace CasinoAppBackend.DTO.Games.HighLow
{
    public class StartRoundResultResponseDTO
    {
        public bool IsWin { get; set; }
        public decimal WinAmount { get; set; }
        public int PreviousCardValue { get; set; }
        public SuitType PreviousCardSuit { get; set; }
        public int NewCardValue { get; set; }
        public SuitType NewCardSuit { get; set; }
        public int RoundNumber { get; set; }
    }
}

using HighLowGameApi.Core.Enums;

namespace HighLowGameApi.Data
{
    public class GameRound : ModifiableEntity
    {
        public int RoundNumber { get; set; }
        public int PreviousCardValue { get; set; }
        public SuitType PreviousCardSuit { get; set; }
        public GuessType GuessType { get; set; }
        public decimal BetAmount { get; set; }
        public int NewCardValue { get; set; }
        public SuitType NewCardSuit { get; set; }
        public bool IsWin { get; set; }
        public decimal WinAmount { get; set; }
        public Guid GameSessionId { get; set; }

        public virtual GameSession GameSession { get; set; } = null!;
    }
}

using HighLowGameApi.Core.Enums;

namespace HighLowGameApi.Data
{
    public class GameSession : ModifiableEntity
    {
        public Guid UserId { get; set; }
        public int StartCardValue { get; set; }
        public SuitType StartCardSuit { get; set; }
        public int CurrentCardValue { get; set; }
        public SuitType CurrentCardSuit { get; set; }
        public GameStatus GameStatus { get; set; }

        public virtual ICollection<GameRound> GameRounds { get; set; } = new HashSet<GameRound>();
    }
}

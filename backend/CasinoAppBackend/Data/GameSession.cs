using CasinoAppBackend.Core.Enums.Games.LowHigh;

namespace CasinoAppBackend.Data
{
    public class GameSession : ModifiableEntity
    {
        public Guid ExternalSessionId { get; set; }
        public GameStatus GameStatus { get; set; } = GameStatus.Active;
        public DateTime? EndedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Game Game { get; set; } = null!;
    }
}

namespace CasinoAppBackend.Data
{
    public class Game : ModifiableEntity
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public bool IsEnabled { get; set; } = true;
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public string Category { get; set; } = null!;
    }
}

namespace CasinoAppBackend.Data.Seeders
{
    public static class GameSeeder
    {
        public static void SeedGame(this CasinoAppBackendDbContext context)
        {
            if (!context.Games.Any())
            {

                var game = new Game
                {
                    Name = "High Low",
                    Code = "highlow",
                    Description = "Guess whether the next card will be higher, lower or equal.",
                    ImageUrl = "https://localhost:5001/images/games/highlow.png",
                    IsEnabled = true,
                    MinBet = 0.10m,
                    MaxBet = 2.00m,
                    Category = "Card"
                };

                context.Games.Add(game);
                context.SaveChanges();
            }
        }
    }
}

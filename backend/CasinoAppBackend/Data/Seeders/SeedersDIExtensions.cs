namespace CasinoAppBackend.Data.Seeders
{
    public static class SeedersDIExtensions
    {
        public static void SeedAll(this CasinoAppBackendDbContext context)
        {
            context.SeedSettings();
            context.SeedSuperAdmin();
            context.SeedGame();
            context.SeedCountries();
        }
    }
}

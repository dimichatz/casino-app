using System.Text.Json;
namespace CasinoAppBackend.Data.Seeders
{
    public static class CountriesSeeder
    {
        public static void SeedCountries(this CasinoAppBackendDbContext context)
        {
            if (context.Countries.Any())
                return;

            var jsonPath = Path.Combine(
                AppContext.BaseDirectory,
                "Resources",
                "countries.json"
            );

            if (!File.Exists(jsonPath))
                throw new FileNotFoundException($"countries.json not found at {jsonPath}");

            var json = File.ReadAllText(jsonPath);
            var countries = JsonSerializer.Deserialize<List<Country>>(json);

            if (countries == null || countries.Count == 0)
                return;

            context.Countries.AddRange(countries);
            context.SaveChanges();
        }
    }
}

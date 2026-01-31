namespace CasinoAppBackend.Data
{
    public class Country
    {
        public string CountryCode { get; set; } = null!;
        public string CountryName { get; set; } = null!;

        public virtual ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}

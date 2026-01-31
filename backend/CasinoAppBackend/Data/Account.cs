namespace CasinoAppBackend.Data
{
    public class Account : ModifiableEntity
    {
        public decimal Balance { get; set; }
        public string Currency { get; set; } = null!;
        public Guid PlayerId { get; set; }
        public virtual Player Player { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}

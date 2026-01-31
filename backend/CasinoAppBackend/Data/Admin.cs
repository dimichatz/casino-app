namespace CasinoAppBackend.Data
{
    public class Admin : ModifiableEntity
    {
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}

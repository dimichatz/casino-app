namespace CasinoAppBackend.Data
{
    public class CreatableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset InsertedAt { get; set; }
    }
}

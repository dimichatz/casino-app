namespace CasinoAppBackend.Data
{
    public class ModifiableEntity : CreatableEntity
    {
        public DateTimeOffset ModifiedAt { get; set; }
    }
}

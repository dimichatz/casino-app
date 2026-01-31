namespace HighLowGameApi.Data
{
    public class ModifiableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime InsertedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}

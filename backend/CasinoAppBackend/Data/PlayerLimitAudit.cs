namespace CasinoAppBackend.Data
{
    public class PlayerLimitAudit : CreatableEntity
    {
        public string FieldName { get; set; } = null!;
        public decimal OldLimit { get; set; }
        public decimal NewLimit { get; set; }
        public Guid PlayerId { get; set; }

        public virtual Player Player { get; set; } = null!;
    }
}

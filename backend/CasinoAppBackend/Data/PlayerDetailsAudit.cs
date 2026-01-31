namespace CasinoAppBackend.Data
{
    public class PlayerDetailsAudit : CreatableEntity
    {
        public Guid ChangedByUserId { get; set; }
        public string ChangedByUsername { get; set; } = null!;
        public string FieldName { get; set; } = null!;
        public string OldValue { get; set; } = null!;
        public string NewValue { get; set; } = null!;
        public string? Comment { get; set; }
        public Guid PlayerId { get; set; }

        public virtual Player Player { get; set; } = null!;
    }
}

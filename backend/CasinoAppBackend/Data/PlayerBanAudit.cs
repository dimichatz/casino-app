namespace CasinoAppBackend.Data
{
    public class PlayerBanAudit : CreatableEntity
    {
        public Guid ChangedByUserId { get; set; }
        public string ChangedByUsername { get; set; } = null!;
        public bool IsBanned { get; set; }
        public string? Comment { get; set; }
        public Guid PlayerId { get; set; }

        public virtual Player Player { get; set; } = null!;
        public virtual User ChangedByUser { get; set; } = null!;

    }
}

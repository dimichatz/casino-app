namespace CasinoAppBackend.DTO.AuditReadOnlyDTO
{
    public class PlayerBanAuditReadOnlyDTO
    {
        public Guid Id { get; set; }
        public Guid ChangedByUserId { get; set; }
        public string ChangedByUsername { get; set; } = null!;
        public bool IsBanned { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset InsertedAt { get; set; }
    }
}

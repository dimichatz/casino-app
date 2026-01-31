namespace CasinoAppBackend.DTO.AuditReadOnlyDTO
{
    public class PlayerDetailsAuditReadOnlyDTO
    {
        public Guid Id { get; set; }
        public Guid ChangedByUserId { get; set; }
        public string ChangedByUsername { get; set; } = null!;
        public string FieldName { get; set; } = null!;
        public string OldValue { get; set; } = null!;
        public string NewValue { get; set; } = null!;
        public string? Comment { get; set; }
        public DateTimeOffset InsertedAt { get; set; }
    }
}

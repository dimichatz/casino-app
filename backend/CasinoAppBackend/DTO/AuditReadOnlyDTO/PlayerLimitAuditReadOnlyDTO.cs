namespace CasinoAppBackend.DTO.AuditReadOnlyDTO
{
    public class PlayerLimitAuditReadOnlyDTO
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; } = null!;
        public decimal OldLimit { get; set; }
        public decimal NewLimit { get; set; }
        public DateTimeOffset InsertedAt { get; set; }
    }
}

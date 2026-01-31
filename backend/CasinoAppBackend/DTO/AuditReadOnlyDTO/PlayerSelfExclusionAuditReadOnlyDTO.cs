using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO.AuditReadOnlyDTO
{
    public class PlayerSelfExclusionAuditReadOnlyDTO
    {
        public Guid Id { get; set; }
        public DateTimeOffset? SelfExclusionStart { get; set; }
        public DateTimeOffset? SelfExclusionEnd { get; set; }
        public SelfExclusionPeriod? SelfExclusionPeriod { get; set; }
        public DateTimeOffset InsertedAt { get; set; }
    }
}

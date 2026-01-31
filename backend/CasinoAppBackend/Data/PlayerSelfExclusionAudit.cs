using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.Data
{
    public class PlayerSelfExclusionAudit : CreatableEntity
    {
        public DateTimeOffset? SelfExclusionStart { get; set; }
        public DateTimeOffset? SelfExclusionEnd { get; set; }
        public SelfExclusionPeriod? SelfExclusionPeriod { get; set; }
        public Guid PlayerId { get; set; }

        public virtual Player Player { get; set; } = null!;
    }
}

using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.Data
{
    public class KycDocument : ModifiableEntity
    {
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = null!;
        public DateTimeOffset? ExpireDate { get; set; }
        public KycStatus KycStatus { get; set; } = KycStatus.Pending;
        public DateTimeOffset? KycCheckDate { get; set; }
        public string? KycCheckedBy { get; set; }
        public Guid PlayerId { get; set; }

        public virtual Player Player { get; set; } = null!;
        public virtual Attachment Attachment { get; set; } = null!;
    }
}

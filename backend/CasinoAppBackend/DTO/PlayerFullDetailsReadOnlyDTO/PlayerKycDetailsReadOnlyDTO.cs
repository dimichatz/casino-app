using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO
{
    public class PlayerKycDetailsReadOnlyDTO
    {
        public bool? IsKycVerified { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public DateTimeOffset? ExpireDate { get; set; }
        public KycStatus? KycStatus { get; set; }
        public DateTimeOffset? KycCheckDate { get; set; }
        public string? KycCheckedBy { get; set; }
    }
}

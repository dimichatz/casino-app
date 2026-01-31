using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO
{
    public class PlayerReadOnlyDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole UserRole { get; set; }
        public bool IsActive { get; set; }
        public KycStatus KycStatus { get; set; }
        public bool IsKycVerified { get; set; }
        public bool IsSelfExcluded { get; set; }
        public DateTimeOffset InsertedAt { get; set; }
    }
}

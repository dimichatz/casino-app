using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO
{
    public class PlayerUserDetailsReadOnlyDTO
    {
        public Guid UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole? UserRole { get; set; }
        public bool? IsActive { get; set; }
    }
}

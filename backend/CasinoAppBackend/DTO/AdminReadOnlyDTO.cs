using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO
{
    public class AdminReadOnlyDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public UserRole UserRole { get; set; }
        public bool IsActive { get; set; }
        public DateTime InsertedAt { get; set; }
    }
}

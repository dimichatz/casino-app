using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.Data
{
    public class User : ModifiableEntity
    {
        public string Username { get; init; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public UserRole UserRole { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual Player? Player { get; set; }
        public virtual Admin? Admin { get; set; }
    }
}

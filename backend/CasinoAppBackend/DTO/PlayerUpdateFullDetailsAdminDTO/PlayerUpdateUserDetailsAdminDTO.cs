using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.DTO.PlayerUpdateFullDetailsAdminDTO
{
    public class PlayerUpdateUserDetailsAdminDTO
    {
        [StringLength(60, ErrorMessage = "Email must not exceed 60 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [RegularExpression(@"^\+3069\d{8}$", ErrorMessage = "Phone number must be a greek mobile number.")]
        public string? PhoneNumber { get; set; }
    }
}

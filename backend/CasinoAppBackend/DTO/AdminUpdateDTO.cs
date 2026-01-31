using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.DTO
{
    public class AdminUpdateDTO
    {
        [StringLength(60, ErrorMessage = "Email must not exceed 60 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [StringLength(60, MinimumLength = 2, ErrorMessage = "Firstname must be between 2 and 60 characters.")]
        public string? Firstname { get; set; }

        [StringLength(60, MinimumLength = 2, ErrorMessage = "Lastname must be between 2 and 60 characters.")]
        public string? Lastname { get; set; }

        [StringLength(25, MinimumLength = 2, ErrorMessage = "Phone number must be between 2 and 25 characters.")]
        public string? PhoneNumber { get; set; }
    }
}

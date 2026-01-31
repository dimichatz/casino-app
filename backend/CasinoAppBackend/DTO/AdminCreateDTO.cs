using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.DTO
{
    public class AdminCreateDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 20 characters.")]
        [RegularExpression("^[a-z0-9]*$", ErrorMessage = "Username must contain only lowercase letters and numbers.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, " +
            "one digit, and one special character.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [StringLength(60, ErrorMessage = "Email must not exceed 60 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Firstname must be between 2 and 60 characters.")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Lastname must be between 2 and 60 characters.")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "Phone number must be between 2 and 25 characters.")]
        public string? PhoneNumber { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.DTO
{
    public class UserChangePasswordDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,50}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, " +
            "one digit, and one special character.")]
        public string? NewPassword { get; set; }
    }
}

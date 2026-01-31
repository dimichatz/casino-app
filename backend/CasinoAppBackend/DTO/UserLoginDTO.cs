using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.DTO
{
    public class UserLoginDTO
    {
        [Required]
        [StringLength(60, MinimumLength =2, ErrorMessage = "Username or Email must be between 2 and 60 characters.")]
        public string UsernameOrEmail { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}

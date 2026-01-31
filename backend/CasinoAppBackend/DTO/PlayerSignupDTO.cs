using System.ComponentModel.DataAnnotations;
using CasinoAppBackend.Attributes;
using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO
{
    public class PlayerSignUpDTO
    {
        [Required(ErrorMessage = "The {0} field is required")]
        [StringLength(60, ErrorMessage = "Email must not exceed 60 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 20 characters.")]
        [RegularExpression("^[a-z0-9]*$", ErrorMessage = "Username must contain only lowercase letters and numbers.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,50}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, " +
            "one digit, and one special character.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }


        [Required(ErrorMessage = "The {0} field is required.")]
        [RegularExpression(@"^\+3069\d{8}$", ErrorMessage = "Phone number must be a greek mobile number.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Firstname must be between 2 and 60 characters.")]
        [RegularExpression("^[A-Za-z -]+$", ErrorMessage = "Latin characters only.")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Lastname must be between 2 and 60 characters.")]
        [RegularExpression("^[A-Za-z -]+$", ErrorMessage = "Latin characters only.")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [EnumDataType(typeof(Gender), ErrorMessage = "Invalid gender.")]
        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [MinimumAge(21)]
        [MaximumAge(100)]
        public DateTime? BirthDate { get; set; }


        [Required(ErrorMessage = "The {0} field is required.")]
        [EnumDataType(typeof(DocumentType), ErrorMessage = "Invalid document type.")]
        public DocumentType DocumentType { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(25, MinimumLength = 5, ErrorMessage = "Document number must be between 5 and 25 characters.")]
        [RegularExpression("^[A-Za-z0-9 -]+$", ErrorMessage = "Latin characters only.")]
        public string? DocumentNumber { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Streetname must be between 2 and 100 characters.")]
        [RegularExpression("^[A-Za-z0-9 -]+$", ErrorMessage = "Latin characters only.")]
        public string? StreetName { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Streetnumber must be between 1 and 20 characters.")]
        [RegularExpression("^[A-Za-z0-9 -]+$", ErrorMessage = "Latin characters only.")]
        public string? StreetNumber { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Postal code must be between 4 and 10 characters.")]
        [RegularExpression("^[A-Za-z0-9 -]+$", ErrorMessage = "Latin characters only.")]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "City must be between 2 and 60 characters.")]
        [RegularExpression("^[A-Za-z -]+$", ErrorMessage = "Latin characters only.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [CountryCode(ErrorMessage = "Invalid country code.")]
        public string? CountryCode { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must confirm that you are of a legal age.")]
        public bool IsAgeVerified { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions.")]
        public bool HasAcceptedTerms { get; set; }   
    }
}

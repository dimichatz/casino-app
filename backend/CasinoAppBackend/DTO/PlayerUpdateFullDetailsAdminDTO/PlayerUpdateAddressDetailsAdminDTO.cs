using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.DTO.PlayerUpdateFullDetailsAdminDTO
{
    public class PlayerUpdateAddressDetailsAdminDTO
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Streetname must be between 2 and 100 characters.")]
        public string? StreetName { get; set; }

        [StringLength(20, MinimumLength = 1, ErrorMessage = "Streetnumber must be between 1 and 20 characters.")]
        public string? StreetNumber { get; set; }

        [StringLength(10, MinimumLength = 4, ErrorMessage = "Postal code must be between 4 and 10 characters.")]
        public string? PostalCode { get; set; }

        [StringLength(60, MinimumLength = 2, ErrorMessage = "City must be between 2 and 60 characters.")]
        public string? City { get; set; }
    }
}

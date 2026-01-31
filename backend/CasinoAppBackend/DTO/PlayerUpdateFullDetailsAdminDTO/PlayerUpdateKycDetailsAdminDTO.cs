using System.ComponentModel.DataAnnotations;
using CasinoAppBackend.Attributes;
using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO.PlayerUpdateFullDetailsAdminDTO
{
    public class PlayerUpdateKycDetailsAdminDTO
    {
        public bool? IsKycVerified { get; set; }

        [StringLength(60, MinimumLength = 2, ErrorMessage = "Firstname must be between 2 and 60 characters.")]
        public string? Firstname { get; set; }

        [StringLength(60, MinimumLength = 2, ErrorMessage = "Lastname must be between 2 and 60 characters.")]
        public string? Lastname { get; set; }

        [EnumDataType(typeof(Gender), ErrorMessage = "Invalid gender.")]
        public Gender? Gender { get; set; }

        [MinimumAge(21)]
        public DateTime? BirthDate { get; set; }

        [EnumDataType(typeof(DocumentType), ErrorMessage = "Invalid document type.")]
        public DocumentType DocumentType { get; set; }

        [StringLength(25, MinimumLength = 5, ErrorMessage = "Document number must be between 5 and 25 characters.")]
        public string? DocumentNumber { get; set; }

        [FutureDate(ErrorMessage= "Date must be in the future.")]
        public DateTime? ExpireDate { get; set; }

        [EnumDataType(typeof(KycStatus), ErrorMessage = "Invalid status.")]
        public KycStatus? KycStatus { get; set; }

        public string? RejectionComment { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (value is not DateTime dateTime)
                return new ValidationResult("Invalid date format");

            return dateTime.Date > DateTime.Today ? ValidationResult.Success 
                : new ValidationResult(ErrorMessage);
        }
    }
}

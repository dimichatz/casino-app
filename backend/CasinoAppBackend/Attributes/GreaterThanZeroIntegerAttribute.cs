using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GreaterThanZeroIntegerAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is not int i)
                return new ValidationResult("Value must be a whole number.");

            return i <= 0 ? new ValidationResult($"{validationContext.DisplayName} must be greater than zero.")
                : ValidationResult.Success;
        }
    }
}

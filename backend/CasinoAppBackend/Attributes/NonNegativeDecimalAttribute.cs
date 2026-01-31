using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NonNegativeDecimalAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not decimal d)
                return new ValidationResult("Value must be a decimal number.");

            return d < 0 ? new ValidationResult($"{validationContext.DisplayName} cannot be negative.")
                : ValidationResult.Success;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CountryCodeAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> ValidCodes =
        [
            "GR"
        ];

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return (value is string code && ValidCodes.Contains(code.ToUpper()))?
                ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}

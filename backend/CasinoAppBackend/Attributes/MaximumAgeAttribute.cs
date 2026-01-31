using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MaximumAgeAttribute : ValidationAttribute
    {
        private readonly int _maximumAge;

        public MaximumAgeAttribute(int maximumAge)
        {
            _maximumAge = maximumAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime birthDate)
                return new ValidationResult("Invalid date format");

            var currentDay = DateTime.Today;
            int age = currentDay.Year - birthDate.Year;

            if (birthDate.Date > currentDay.AddYears(-age)) 
                age--;

            return age <= _maximumAge ? ValidationResult.Success
                : new ValidationResult($"Age must not be greater than {_maximumAge} years.");
        }
    }
}

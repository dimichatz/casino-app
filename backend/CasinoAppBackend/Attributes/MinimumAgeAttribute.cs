using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime birthDate)
                return new ValidationResult("Invalid date format");

            var currentDay = DateTime.Today;
            int age = currentDay.Year - birthDate.Year;

            if (birthDate.Date > currentDay.AddYears(-age)) 
                age--;

            return age >= _minimumAge ? ValidationResult.Success
                :new ValidationResult($"You must be at least {_minimumAge} years old.");
        }
    }
}

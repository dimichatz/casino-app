using System.ComponentModel.DataAnnotations;

namespace CasinoAppBackend.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequireBothDatesAttribute : ValidationAttribute
    {
        public string OtherProperty { get; }

        public RequireBothDatesAttribute(string otherProperty)
        {
            OtherProperty = otherProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentProperty = value as DateTime?;
            var otherProperty = validationContext.ObjectType.GetProperty(OtherProperty)?.
                GetValue(validationContext.ObjectInstance) as DateTime?;

            if (currentProperty.HasValue && !otherProperty.HasValue)
                return new ValidationResult("Both dates must be provided.");

            return ValidationResult.Success;
        }
    }
}
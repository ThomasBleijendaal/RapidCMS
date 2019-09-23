using System;
using System.ComponentModel.DataAnnotations;

namespace TestLibrary.Validation
{
    public class TrueAttribute : ValidationAttribute 
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is bool boolValue)
            {
                if (boolValue != true)
                {
                    return new ValidationResult($"The field {validationContext.DisplayName} must be True.", new[] { validationContext.MemberName });
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                throw new InvalidOperationException($"{nameof(TrueAttribute)} must be used on a bool.");
            }
        }

        public override bool RequiresValidationContext => true;
    }
}

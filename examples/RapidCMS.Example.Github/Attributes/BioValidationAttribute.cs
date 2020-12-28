using System.ComponentModel.DataAnnotations;

namespace RapidCMS.Example.Github.Attributes
{
    internal class BioValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string bio)
            {
                if (bio.Contains("fdsa"))
                {
                    return new ValidationResult("Bio's cannot contain 'fdsa'.", validationContext.MemberName == null ? null : new[] { validationContext.MemberName });
                }
            }

            return ValidationResult.Success!;
        }
    }
}

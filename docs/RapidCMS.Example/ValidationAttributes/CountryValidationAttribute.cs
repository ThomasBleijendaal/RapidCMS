using System.ComponentModel.DataAnnotations;

namespace RapidCMS.Example.ValidationAttributes
{
    public class CountryValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return default;
        }
    }

    public class CountryMetadataValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return new ValidationResult("BORK");
        }
    }
}

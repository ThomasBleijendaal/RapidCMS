using System.ComponentModel.DataAnnotations;
using RapidCMS.Example.Shared.Data;
using static RapidCMS.Example.Shared.Data.Country;

namespace RapidCMS.Example.Shared.ValidationAttributes
{
    public class CountryValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is Country country)
            {
                if (country.Name == "fdsa")
                {
                    return new ValidationResult("The name of the country cannot be 'fdsa'.");
                }
            }

            return ValidationResult.Success;
        }
    }

    public class CountryMetadataValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is CountryMetadata metadata)
            {
                if (metadata.Continent == "fdsafdsa")
                {
                    return new ValidationResult("The name of the country's continent cannot be 'fdsafdsa'.");
                }
            }

            return ValidationResult.Success;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace TestLibrary.Validation
{
    public class EntityValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return new ValidationResult("Entity invalid", new[] { "Name" });
        }

        public override bool RequiresValidationContext => true;
    }
}

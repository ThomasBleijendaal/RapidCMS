using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RapidCMS.Core.Attributes;

/// <summary>
/// This attribute will instruct the entity validator to validate the properties of this object, and use them as validation result of the object.
/// 
/// Use this attribute when using a ModelEditor, or a custom editor which requires objects.
/// </summary>
public class ValidateObjectAsPropertyAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success!;
        }

        var results = new List<ValidationResult>();
        var context = new ValidationContext(value, validationContext, null);

        Validator.TryValidateObject(value, context, results, true);

        if (results.Count != 0)
        {
            return new ValidationResult(
                string.Join(" ", results.Select(x => x.ErrorMessage)),
                !string.IsNullOrEmpty(validationContext.MemberName) ? new[] { validationContext.MemberName } : null);
        }

        return ValidationResult.Success!;
    }
}

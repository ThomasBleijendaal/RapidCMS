using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Models;

namespace RapidCMS.Core.Attributes;

/// <summary>
/// This attribute will instruct the entity validator to also validate the properties of this object, instead of just the object itself.
/// 
/// Use this attribute when configuring nested properties, like: config.AddField(x => x.Object.Property).
/// </summary>
public class ValidateObjectAttribute : ValidationAttribute
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
            var compositeResults = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!", validationContext.MemberName ?? "Unknown member");
            results.ForEach(compositeResults.AddResult);

            return compositeResults;
        }

        return ValidationResult.Success!;
    }
}

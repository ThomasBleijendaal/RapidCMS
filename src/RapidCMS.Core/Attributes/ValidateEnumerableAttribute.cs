using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Models;

namespace RapidCMS.Core.Attributes
{
    /// <summary>
    /// This attribute will instruct the entity validator to also validate each of the elements of this enumerable, instead of just the enumerable itself.
    /// </summary>
    public class ValidateEnumerableAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!;
            }

            if (value is not IEnumerable enumerable)
            {
                return new ValidationResult($"{nameof(ValidateEnumerableAttribute)} can only be used on IEnumerable's");
            }

            var results = new List<ValidationResult>();

            foreach (var item in enumerable)
            {
                var context = new ValidationContext(item, validationContext, null);

                Validator.TryValidateObject(item, context, results, true);
            }

            var compositeResults = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!", validationContext.MemberName ?? "Unknown member");
            if (results.Count != 0)
            {
                results.ForEach(compositeResults.AddResult);
                return compositeResults;
            }

            return ValidationResult.Success!;
        }
    }
}

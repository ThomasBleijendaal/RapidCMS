using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms.Validation;

namespace RapidCMS.Example.Shared.ValidationAttributes
{
    public class MaxTwoAttribute : RelationValidationAttribute
    {
        public override ValidationResult? IsValid(IEntity entity, IReadOnlyList<object> relatedElementIds, ValidationContext validationContext)
        {
            if (relatedElementIds.Count() <= 2)
            {
                return null;
            }

            return new ValidationResult("This properties can only have 2 related entities.");
        }
    }
}

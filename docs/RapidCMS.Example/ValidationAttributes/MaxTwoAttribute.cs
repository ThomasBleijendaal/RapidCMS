using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;

namespace RapidCMS.Example.ValidationAttributes
{
    public class MaxTwoAttribute : RelationValidationAttribute
    {
        public override ValidationResult? IsValid(IEntity entity, IEnumerable<IElement> relatedElements, ValidationContext validationContext)
        {
            if (relatedElements.Count() <= 2)
            {
                return null;
            }

            return new ValidationResult("This properties can only have 2 related entities.");
        }
    }
}

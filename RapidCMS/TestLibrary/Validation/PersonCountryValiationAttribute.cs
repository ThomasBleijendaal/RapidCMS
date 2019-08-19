using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;

namespace TestLibrary.Validation
{
    public class PersonCountryValiationAttribute : RelationValidationAttribute
    {
        public override ValidationResult? IsValid(IEntity entity, IEnumerable<IElement> relatedElements, ValidationContext validationContext)
        {
            if (!relatedElements.Count().In(2, 3))
            {
                return new ValidationResult("A person must have 2 or 3 countries.", new[] { validationContext.MemberName });
            }

            return null;
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RapidCMS.Common.Data
{
    public interface IRelationValidator
    {
        public IEnumerable<ValidationResult> Validate(IEntity entity, IEnumerable<IRelatedElement> relatedElements);
    }
}

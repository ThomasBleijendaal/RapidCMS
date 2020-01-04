using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Validators
{
    public interface IRelationValidator
    {
        public IEnumerable<ValidationResult> Validate(IEntity entity, IEnumerable<IElement> relatedElements, IServiceProvider serviceProvider);
    }
}

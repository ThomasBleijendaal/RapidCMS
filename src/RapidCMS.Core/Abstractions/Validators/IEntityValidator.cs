using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Validators
{
    public interface IEntityValidator
    {
        IEnumerable<ValidationResult> Validate(IEntity entity);
    }
}

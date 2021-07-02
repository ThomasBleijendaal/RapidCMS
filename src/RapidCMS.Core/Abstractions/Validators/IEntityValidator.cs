using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Validators
{
    public interface IEntityValidator
    {
        IEnumerable<ValidationResult> Validate(IValidatorContext context);
    }

    public interface IEntityValidator<TEntity> : IEntityValidator
        where TEntity : IEntity
    {
        IEnumerable<ValidationResult> Validate(IValidatorContext<TEntity> context);
    }
}

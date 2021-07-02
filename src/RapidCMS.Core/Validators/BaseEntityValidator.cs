using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Validators;

namespace RapidCMS.Core.Validators
{
    public abstract class BaseEntityValidator<TEntity> : IEntityValidator
        where TEntity : IEntity
    {
        public abstract IEnumerable<ValidationResult> Validate(IValidatorContext<TEntity> context);

        IEnumerable<ValidationResult> IEntityValidator.Validate(IValidatorContext context) 
            => Validate(new ValidatorContext<TEntity>(context.Entity, context.RelationContainer, context.Configuration));
    }
}

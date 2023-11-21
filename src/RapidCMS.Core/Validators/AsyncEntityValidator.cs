using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Validators;

namespace RapidCMS.Core.Validators;

public abstract class BaseAsyncEntityValidator<TEntity> : IAsyncEntityValidator, IAsyncEntityValidator<TEntity>
    where TEntity : IEntity
{
    public abstract Task<IEnumerable<ValidationResult>> ValidateAsync(IValidatorContext<TEntity> context);

    Task<IEnumerable<ValidationResult>> IAsyncEntityValidator.ValidateAsync(IValidatorContext context)
        => ValidateAsync(new ValidatorContext<TEntity>(context.Entity, context.RelationContainer, context.Configuration));
}

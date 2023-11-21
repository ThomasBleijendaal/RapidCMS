using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Validators;

public interface IAsyncEntityValidator
{
    Task<IEnumerable<ValidationResult>> ValidateAsync(IValidatorContext context);
}

public interface IAsyncEntityValidator<TEntity> : IAsyncEntityValidator
    where TEntity : IEntity
{
    Task<IEnumerable<ValidationResult>> ValidateAsync(IValidatorContext<TEntity> context);
}

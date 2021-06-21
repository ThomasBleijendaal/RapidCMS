using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Validators;

namespace RapidCMS.Core.Validators
{
    public abstract class BaseAsyncEntityValidator<TEntity> : IAsyncEntityValidator
        where TEntity : IEntity
    {
        protected abstract Task<IEnumerable<ValidationResult>> ValidateAsync(TEntity entity);

        Task<IEnumerable<ValidationResult>> IAsyncEntityValidator.ValidateAsync(IEntity entity) => ValidateAsync((TEntity)entity);
    }
}

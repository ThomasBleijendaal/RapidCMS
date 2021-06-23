using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Validators;

namespace RapidCMS.Example.Shared.Validators
{
    /// <summary>
    /// This adapter adapts FluentValidation's AbstractValidator to TEntity part of IEntityValidator
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class AbstractValidatorAdapter<TEntity> : AbstractValidator<TEntity>, IEntityValidator
        where TEntity : IEntity
    {
        IEnumerable<ValidationResult> IEntityValidator.Validate(IEntity entity, IRelationContainer relationContainer)
        {
            var result = Validate((TEntity)entity);

            if (result.IsValid)
            {
                return Enumerable.Empty<ValidationResult>();
            }
            else
            {
                return result.Errors.Select(error => new ValidationResult(error.ErrorMessage, new[] { error.PropertyName }));
            }
        }
    }
}

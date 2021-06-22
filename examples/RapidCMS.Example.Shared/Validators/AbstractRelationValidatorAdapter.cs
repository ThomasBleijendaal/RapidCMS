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
    /// This adapter adapts FluentValidation's AbstractValidator to the IRelationContainer part of IEntityValidator
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class AbstractRelationValidatorAdapter<TEntity> : AbstractValidator<IRelationContainer>, IEntityValidator
        where TEntity : IEntity
    {
        IEnumerable<ValidationResult> IEntityValidator.Validate(IEntity entity, IRelationContainer relationContainer)
        {
            var result = Validate(relationContainer);

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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Validators;

namespace RapidCMS.Example.Shared.Validators
{
    /// <summary>
    /// This adapter adapts FluentValidation's AbstractValidator to TEntity part of IEntityValidator
    /// 
    /// Since the context is saved as property these validators must be added as transient services.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class AbstractValidatorAdapter<TEntity> : AbstractValidator<TEntity>, IEntityValidator
        where TEntity : IEntity
    {
        protected IValidatorContext _context = default!;

        IEnumerable<ValidationResult> IEntityValidator.Validate(IValidatorContext context)
        {
            _context = context;

            var result = Validate((TEntity)context.Entity);

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

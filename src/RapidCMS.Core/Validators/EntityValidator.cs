using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Validators;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Validators;

internal static class EntityValidator
{
    public static async IAsyncEnumerable<ValidationResult> ValidateAsync(
        IReadOnlyList<ValidationSetup> validators,
        IRelationContainer relationContainer,
        IServiceProvider serviceProvider,
        IEntity entity,
        string? propertyName = default)
    {
        foreach (var validatorSetup in validators)
        {
            var isVariantValidator = validatorSetup.Type.HasInterface(typeof(IEntityValidator<>).MakeGenericType(entity.GetType())) ||
                validatorSetup.Type.HasInterface(typeof(IAsyncEntityValidator<>).MakeGenericType(entity.GetType()));

            var isGenericVariantValidator = validatorSetup.Type.HasInterface(typeof(IEntityValidator<IEntity>)) ||
                validatorSetup.Type.HasInterface(typeof(IAsyncEntityValidator<IEntity>));

            var isVariantValidatorForOtherEntity = validatorSetup.Type.HasGenericInterface(typeof(IEntityValidator<>)) ||
                validatorSetup.Type.HasGenericInterface(typeof(IAsyncEntityValidator<>));

            var isGenericValidator = validatorSetup.Type.HasInterface(typeof(IEntityValidator)) ||
                validatorSetup.Type.HasInterface(typeof(IAsyncEntityValidator));

            if (isVariantValidator || isGenericVariantValidator || (!isVariantValidator && !isVariantValidatorForOtherEntity && isGenericValidator))
            {
                var validationResult = serviceProvider.GetService(validatorSetup.Type) switch
                {
                    IEntityValidator validator => validator.Validate(new ValidatorContext(entity, relationContainer, validatorSetup.Configuration)),
                    IAsyncEntityValidator asyncValidator => await asyncValidator.ValidateAsync(new ValidatorContext(entity, relationContainer, validatorSetup.Configuration)),
                    _ => throw new InvalidOperationException($"Invalid entity validator given. {validatorSetup.Type.Name} is not included in Service Collection or is not a {typeof(IEntityValidator).Name} or {typeof(IAsyncEntityValidator).Name}.")
                };

                foreach (var result in FlattenCompositeValidationResults(validationResult).Where(x => string.IsNullOrWhiteSpace(propertyName) || x.MemberNames.Contains(propertyName)))
                {
                    yield return result;
                }
            }
        }
    }

    private static IEnumerable<ValidationResult> FlattenCompositeValidationResults(IEnumerable<ValidationResult> results, string? memberNamePrefix = null)
    {
        foreach (var result in results)
        {
            // TODO: even after disabling the DataAnnotationValidator, entity validation is still dependent on some Attributes to work correctly
            if (result is CompositeValidationResult composite)
            {
                foreach (var nestedResult in FlattenCompositeValidationResults(composite.Results, composite.MemberName))
                {
                    yield return nestedResult;
                }
            }
            else if (string.IsNullOrWhiteSpace(memberNamePrefix))
            {
                yield return result;
            }
            else
            {
                yield return new ValidationResult(result.ErrorMessage, result.MemberNames.Select(x => $"{memberNamePrefix}.{x}"));
            }

        }
    }
}

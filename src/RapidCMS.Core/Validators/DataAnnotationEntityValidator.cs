using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Validators;

namespace RapidCMS.Core.Validators
{
    public class DataAnnotationEntityValidator : BaseEntityValidator<IEntity>
    {
        private readonly IServiceProvider _serviceProvider;

        public DataAnnotationEntityValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IEnumerable<ValidationResult> Validate(IValidatorContext<IEntity> context)
        {
            var results = new List<ValidationResult>();

            try
            {
                var ctx = new ValidationContext(context.Entity, _serviceProvider, null);
                // even though this says Try, and therefore it should not throw an error, IT DOES when a given property is not part of Entity
                Validator.TryValidateObject(context.Entity, ctx, results, true);
            }
            catch { }

            return results;
        }
    }
}

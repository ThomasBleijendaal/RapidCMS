using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMS.Core.Validators
{
    public class DataAnnotationEntityValidator : BaseEntityValidator<IEntity>
    {
        private readonly IServiceProvider _serviceProvider;

        public DataAnnotationEntityValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override IEnumerable<ValidationResult> Validate(IEntity entity, IRelationContainer relationContainer)
        {
            var results = new List<ValidationResult>();

            try
            {
                var context = new ValidationContext(entity, _serviceProvider, null);
                // even though this says Try, and therefore it should not throw an error, IT DOES when a given property is not part of Entity
                Validator.TryValidateObject(entity, context, results, true);
            }
            catch { }

            return results;
        }
    }
}

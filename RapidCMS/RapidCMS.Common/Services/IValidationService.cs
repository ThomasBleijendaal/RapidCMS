using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Services
{
    [Obsolete("Use EditContext instead")]
    public interface IValidationService
    {
        Task<bool> IsValidAsync(IEntity entity);
    }

    [Obsolete("Use EditContext instead")]
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<bool> IsValidAsync(IEntity entity)
        {
            var context = new ValidationContext(entity, _serviceProvider, null);

            var results = new List<ValidationResult>();

            Validator.TryValidateObject(entity, context, results, true);

            return Task.FromResult(!results.Any());
        }
    }
}

using System.Threading.Tasks;
using RapidCMS.ModelMaker.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Validation.Base
{
    public abstract class BaseValidator<TValue, TValidatorConfig> : IValidator
        where TValidatorConfig : IValidatorConfig
    {
        public Task<string> ErrorMessageAsync(IValidatorConfig validatorConfig)
        {
            if (validatorConfig is TValidatorConfig config)
            {
                return ErrorMessage(config);
            }

            return Task.FromResult("Unknown error");
        }

        public Task<bool> IsValidAsync(object? value, IValidatorConfig validatorConfig)
        {
            if (value is TValue validationValue && validatorConfig is TValidatorConfig config)
            {
                return IsValid(validationValue, config);
            }

            return Task.FromResult(false);
        }

        protected abstract Task<bool> IsValid(TValue? value, TValidatorConfig validatorConfig);
        protected abstract Task<string> ErrorMessage(TValidatorConfig validatorConfig);
    }
}

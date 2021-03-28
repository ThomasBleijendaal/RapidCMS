using System.Threading.Tasks;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class MinLengthValidator : BaseValidator<string, MinLengthValidationConfig>
    {
        protected override Task<string> ErrorMessage(MinLengthValidationConfig validatorConfig)
        {
            return Task.FromResult($"The input has to be at least {validatorConfig.MinLength} characters long.");
        }

        protected override Task<bool> IsValid(string? value, MinLengthValidationConfig validatorConfig)
        {
            return Task.FromResult(value?.Length >= validatorConfig.MinLength);
        }
    }
}

using System.Threading.Tasks;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class LimitedOptionsValidator : BaseValidator<string, LimitedOptionsValidationConfig>
    {
        protected override Task<string> ErrorMessage(LimitedOptionsValidationConfig validatorConfig)
        {
            return Task.FromResult($"The input must be one of these values: {string.Join(", ", validatorConfig.Options)}.");
        }

        protected override Task<bool> IsValid(string? value, LimitedOptionsValidationConfig validatorConfig)
        {
            return Task.FromResult(validatorConfig.Options.Contains(value ?? string.Empty));
        }
    }
}

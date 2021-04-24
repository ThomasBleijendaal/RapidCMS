using System.Threading.Tasks;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class MaxLengthValidator : BaseValidator<string, MaxLengthValidationConfig>
    {
        protected override Task<string> ErrorMessage(MaxLengthValidationConfig validatorConfig)
        {
            return Task.FromResult($"The input has to be at most {validatorConfig.MaxLength} characters long.");
        }

        protected override Task<bool> IsValid(string? value, MaxLengthValidationConfig validatorConfig)
        {
            return Task.FromResult(value?.Length <= validatorConfig.MaxLength);
        }
    }
}

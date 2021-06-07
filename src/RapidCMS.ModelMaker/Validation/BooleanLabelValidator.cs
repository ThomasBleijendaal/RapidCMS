using System.Threading.Tasks;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class BooleanLabelValidator : BaseValidator<bool, BooleanLabelValidationConfig>
    {
        protected override Task<string> ErrorMessage(BooleanLabelValidationConfig validatorConfig) => Task.FromResult("");

        protected override Task<bool> IsValid(bool value, BooleanLabelValidationConfig validatorConfig) => Task.FromResult(true);
    }
}

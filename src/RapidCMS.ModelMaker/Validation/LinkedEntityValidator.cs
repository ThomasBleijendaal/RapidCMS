using System.Threading.Tasks;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class LinkedEntityValidator : BaseValidator<string, LinkedEntityValidationConfig>
    {
        protected override Task<string> ErrorMessage(LinkedEntityValidationConfig validatorConfig)
        {
            return Task.FromResult("");
        }

        protected override Task<bool> IsValid(string? value, LinkedEntityValidationConfig validatorConfig)
        {
            return Task.FromResult(true);
        }
    }
}

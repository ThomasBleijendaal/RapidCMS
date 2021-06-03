using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class LinkedEntitiesValidator : BaseValidator<List<string>, LinkedEntityValidationConfig>
    {
        protected override Task<string> ErrorMessage(LinkedEntityValidationConfig validatorConfig)
        {
            return Task.FromResult("");
        }

        protected override Task<bool> IsValid(List<string>? value, LinkedEntityValidationConfig validatorConfig)
        {
            // TODO: validate picked entities
            return Task.FromResult(true);
        }
    }
}

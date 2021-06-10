using System.Collections.Generic;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class LinkedEntitiesValidator : BaseValidator<List<string>, LinkedEntitiesValidationConfig>
    {
        protected override string? ValidationAttributeText(LinkedEntitiesValidationConfig validatorConfig) => default;
    }
}

using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class LinkedEntityValidator : BaseValidator<string, LinkedEntityValidationConfig>
    {
        protected override string? ValidationAttributeText(LinkedEntityValidationConfig validatorConfig) => default;
    }
}

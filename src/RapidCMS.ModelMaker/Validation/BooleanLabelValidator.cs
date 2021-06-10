using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class BooleanLabelValidator : BaseValidator<bool, BooleanLabelValidationConfig>
    {
        protected override string? ValidationAttributeText(BooleanLabelValidationConfig validatorConfig) => default;
    }
}

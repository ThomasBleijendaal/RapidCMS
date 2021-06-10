using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class MaxLengthValidator : BaseValidator<string, MaxLengthValidationConfig>
    {
        protected override string? ValidationAttributeText(MaxLengthValidationConfig validatorConfig)
            => $"[MaxLength({validatorConfig.MaxLength})]";
    }
}

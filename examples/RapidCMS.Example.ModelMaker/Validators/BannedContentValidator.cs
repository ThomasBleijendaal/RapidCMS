using RapidCMS.ModelMaker.Validation.Base;

namespace RapidCMS.Example.ModelMaker.Validators
{
    public class BannedContentValidator : BaseValidator<string, BannedContentValidationConfig>
    {
        protected override string? ValidationAttributeText(BannedContentValidationConfig validatorConfig)
            => $"[RegularExpression(\"[^{string.Join("|", validatorConfig.BannedWords)}$]\")]";
    }
}

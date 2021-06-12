//using RapidCMS.ModelMaker.Validation.Base;
//using RapidCMS.ModelMaker.Validation.Config;

//namespace RapidCMS.ModelMaker.Validation
//{
//    public class LimitedOptionsValidator : BaseValidator<string, LimitedOptionsValidationConfig>
//    {
//        protected override string? ValidationAttributeText(LimitedOptionsValidationConfig validatorConfig)
//            => $"[RegularExpression(\"^[{string.Join("|", validatorConfig.Options)}]\")]$";
//    }
//}

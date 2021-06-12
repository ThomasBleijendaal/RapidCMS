//using System.Threading.Tasks;
//using RapidCMS.ModelMaker.Core.Abstractions.Validation;

//namespace RapidCMS.ModelMaker.Validation.Base
//{
//    // TODO: Rename
//    public abstract class BaseValidator<TValue, TValidatorConfig> : IValidator
//        where TValidatorConfig : IValidatorConfig
//    {
//        public string? ValidationAttributeText(IValidatorConfig validatorConfig)
//        {
//            if (validatorConfig is TValidatorConfig config)
//            {
//                return ValidationAttributeText(config);
//            }

//            return default;
//        }

//        protected abstract string? ValidationAttributeText(TValidatorConfig validatorConfig);
//    }
//}

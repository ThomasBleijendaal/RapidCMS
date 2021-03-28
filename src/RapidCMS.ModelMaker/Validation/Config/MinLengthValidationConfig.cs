using RapidCMS.ModelMaker.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class MinLengthValidationConfig : IValidatorConfig
    {
        public int? MinLength { get; set; }

        public bool IsEnabled => MinLength.HasValue;
    }
}

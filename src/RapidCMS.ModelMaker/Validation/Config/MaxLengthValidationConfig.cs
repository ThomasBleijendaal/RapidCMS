using RapidCMS.ModelMaker.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class MaxLengthValidationConfig : IValidatorConfig
    {
        public int? MaxLength { get; set; }

        public bool IsEnabled => MaxLength.HasValue;
    }
}

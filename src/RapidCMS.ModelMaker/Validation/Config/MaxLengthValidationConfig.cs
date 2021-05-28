using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class MaxLengthValidationConfig : IValidatorConfig
    {
        public int? MaxLength { get; set; }

        public bool IsEnabled => MaxLength.HasValue;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.TextArea, Constants.Editors.TextBox);
    }
}

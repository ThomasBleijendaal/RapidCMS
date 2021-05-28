using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class MinLengthValidationConfig : IValidatorConfig
    {
        public int? MinLength { get; set; }

        public bool IsEnabled => MinLength.HasValue;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.TextArea, Constants.Editors.TextBox);
    }
}

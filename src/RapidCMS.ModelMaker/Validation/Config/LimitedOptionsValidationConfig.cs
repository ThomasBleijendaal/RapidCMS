using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class LimitedOptionsValidationConfig : IValidatorConfig
    {
        public List<string> Options { get; set; } = new List<string>();

        public bool IsEnabled => Options?.Any() == true;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.Dropdown);
    }
}

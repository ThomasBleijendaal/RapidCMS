using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class LimitedOptionsValidationConfig : IValidatorConfig
    {
        [Required]
        [MinLength(1)]
        public List<string> Options { get; set; } = new List<string>();

        public bool IsEnabled => Options?.Any() == true;
        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.Dropdown);

        public string? RelatedCollectionAlias => default;
    }
}

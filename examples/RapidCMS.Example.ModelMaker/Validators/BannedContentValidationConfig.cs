using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.Example.ModelMaker.Validators
{
    public class BannedContentValidationConfig : IValidatorConfig
    {
        public List<string> BannedWords { get; set; } = new List<string>();

        public bool IsEnabled => BannedWords.Any();

        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.TextBox, Constants.Editors.TextArea);

        public string? RelatedCollectionAlias => default;
    }
}

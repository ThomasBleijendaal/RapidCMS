using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class LinkedEntityValidationConfig : IValidatorConfig
    {
        public string CollectionAlias { get; set; } = string.Empty;

        public bool IsEnabled => !string.IsNullOrWhiteSpace(CollectionAlias);

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.Dropdown, Constants.Editors.Select);
    }
}

using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class LinkedEntitiesValidationConfig : IValidatorConfig
    {
        [Required]
        public string LinkedEntitiesCollectionAlias { get; set; } = string.Empty;

        public bool IsEnabled => !string.IsNullOrWhiteSpace(LinkedEntitiesCollectionAlias);
        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.EntityPicker, Constants.Editors.EntitiesPicker);

        public string? RelatedCollectionAlias => IsEnabled ? LinkedEntitiesCollectionAlias : default;
    }
}

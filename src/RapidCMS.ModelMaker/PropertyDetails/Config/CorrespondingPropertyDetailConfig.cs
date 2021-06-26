using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class CorrespondingPropertyDetailConfig : IDetailConfig
    {
        public string? RelatedPropertyName { get; set; }

        public bool IsEnabled => !string.IsNullOrWhiteSpace(RelatedPropertyName);
        public bool AlwaysIncluded => false;

        public bool IsApplicable(PropertyModel model)
            => model.EditorAlias.In(Constants.Editors.EntityPicker, Constants.Editors.EntitiesPicker);

        public string? RelatedCollectionAlias => default;

        public string? ValidationMethodName => default;

        public string? DataCollectionType => default;
    }
}

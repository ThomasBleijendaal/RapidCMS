using RapidCMS.ModelMaker.Abstractions.Detail;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class DataCollectionConfig<TDataCollection> : IDetailConfig
    {
        public bool IsEnabled => true;

        public bool AlwaysIncluded => true;

        public bool IsApplicable(PropertyModel model) => false;

        public string? RelatedCollectionAlias => default;

        public string? ValidationMethodName => default;

        public string? DataCollectionType => typeof(TDataCollection).FullName;
    }
}

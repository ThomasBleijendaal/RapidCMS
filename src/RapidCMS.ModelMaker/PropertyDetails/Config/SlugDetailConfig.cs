using RapidCMS.ModelMaker.Abstractions.Detail;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Validation.Config
{
    public class SlugDetailConfig : IDetailConfig
    {
        public string? Dummy { get; set; }

        public bool IsEnabled => true;

        public bool AlwaysIncluded => true;

        public bool IsApplicable(PropertyModel model) => true;

        public string? RelatedCollectionAlias => default;

        public string? ValidationMethodName => "UrlFriendly";

        public string? DataCollectionType => default;
    }
}

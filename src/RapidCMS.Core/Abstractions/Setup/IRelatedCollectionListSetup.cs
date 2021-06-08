using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IRelatedCollectionListSetup
    {
        public int Index { get; set; }
        public string CollectionAlias { get; set; }

        public UsageType SupportsUsageType { get; set; }
    }
}

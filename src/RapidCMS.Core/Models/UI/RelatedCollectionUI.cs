using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.UI
{
    public class RelatedCollectionUI : ElementUI
    {
        internal RelatedCollectionUI(IRelatedCollectionListSetup relatedCollection) : base((x, state) => state == EntityState.IsExisting, (x, y) => false)
        {
            CollectionAlias = relatedCollection.CollectionAlias;
            SupportsUsageType = relatedCollection.SupportsUsageType;
        }

        public string CollectionAlias { get; private set; }
    }
}

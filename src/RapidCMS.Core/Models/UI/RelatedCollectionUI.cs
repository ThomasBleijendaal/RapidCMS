using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class RelatedCollectionUI : ElementUI
    {
        internal RelatedCollectionUI(RelatedCollectionListSetup relatedCollection) : base((x, state) => state == EntityState.IsExisting, (x, y) => false)
        {
            CollectionAlias = relatedCollection.CollectionAlias;
            SupportsUsageType = relatedCollection.SupportsUsageType;
        }

        public string CollectionAlias { get; private set; }
    }
}

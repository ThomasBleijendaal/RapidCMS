using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.UI
{
    public class SubCollectionUI : ElementUI
    {
        internal SubCollectionUI(ISubCollectionListSetup subCollection) : base((x, state) => state == EntityState.IsExisting, (x, y) => false)
        {
            CollectionAlias = subCollection.CollectionAlias;
            SupportsUsageType = subCollection.SupportsUsageType;
        }

        public string CollectionAlias { get; private set; }
    }
}

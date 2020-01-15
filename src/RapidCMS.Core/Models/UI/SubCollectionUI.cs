using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class SubCollectionUI : ElementUI
    {
        internal SubCollectionUI(SubCollectionListSetup subCollection) : base((x, state) => state == EntityState.IsExisting, (x, y) => false)
        {
            CollectionAlias = subCollection.CollectionAlias;
        }

        public string CollectionAlias { get; private set; }
    }
}

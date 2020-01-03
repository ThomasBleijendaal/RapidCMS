using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class RelatedCollectionUI : ElementUI
    {
        internal RelatedCollectionUI(RelatedCollectionListSetup relatedCollection) : base((x, y) => true, (x, y) => false)
        {
            CollectionAlias = relatedCollection.CollectionAlias;
        }

        public string CollectionAlias { get; private set; }
    }
}

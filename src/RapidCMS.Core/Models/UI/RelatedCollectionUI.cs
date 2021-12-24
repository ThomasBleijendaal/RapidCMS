using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.UI
{
    public class RelatedCollectionUI : ElementUI
    {
        internal RelatedCollectionUI(IRelatedCollectionListSetup relatedCollection, NavigationState nestedNavigationState) 
            : base((x, state) => state == EntityState.IsExisting, (x, y) => false)
        {
            CollectionAlias = relatedCollection.CollectionAlias;
            NestedNavigationState = nestedNavigationState;
        }

        public string CollectionAlias { get; private set; }

        public NavigationState? NestedNavigationState { get; internal set; }
    }
}

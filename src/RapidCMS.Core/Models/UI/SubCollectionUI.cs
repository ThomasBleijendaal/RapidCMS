using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.UI
{
    public class SubCollectionUI : ElementUI
    {
        internal SubCollectionUI(SubCollectionListSetup subCollection, NavigationState nestedNavigationState) 
            : base((x, state) => state == EntityState.IsExisting, (x, y) => false)
        {
            CollectionAlias = subCollection.CollectionAlias;
            NestedNavigationState = nestedNavigationState;
        }

        public string CollectionAlias { get; private set; }

        public NavigationState? NestedNavigationState { get; private set; }
    }
}

using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.UI
{
    public class SubCollectionUI : ElementUI
    {
        internal SubCollectionUI(ISubCollectionListSetup subCollection, NavigationState nestedNavigationState) 
            : base((x, state) => state == EntityState.IsExisting, (x, y) => false)
        {
            CollectionAlias = subCollection.CollectionAlias;
            NestedNavigationState = nestedNavigationState;
        }

        public string CollectionAlias { get; private set; }

        public NavigationState? NestedNavigationState { get; private set; }
    }
}

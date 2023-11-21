using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.UI;

public class RelatedCollectionUI : ElementUI
{
    internal RelatedCollectionUI(RelatedCollectionListSetup relatedCollection, NavigationState nestedNavigationState) 
        : base((x, state) => state == EntityState.IsExisting, (x, y) => false)
    {
        CollectionAlias = relatedCollection.CollectionAlias;
        NestedNavigationState = nestedNavigationState;
    }

    public string CollectionAlias { get; private set; }

    public NavigationState? NestedNavigationState { get; internal set; }
}

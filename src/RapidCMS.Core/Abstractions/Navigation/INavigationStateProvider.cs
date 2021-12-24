using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Abstractions.Navigation
{
    public interface INavigationStateProvider
    {
        NavigationState GetCurrentState();

        NavigationState Initialize(string url, string queryString);

        void AppendNavigationState(NavigationState state);

        void NestNavigationState(NavigationState state, NavigationState nestedState);

        void ReplaceNavigationState(NavigationState state);

        bool RemoveNavigationState();

        void UpdateCollectionState(CollectionState state);

        IView GetCurrentView(ListUI list);

        bool TryProcessView(IView view, bool hasSections);
    }
}

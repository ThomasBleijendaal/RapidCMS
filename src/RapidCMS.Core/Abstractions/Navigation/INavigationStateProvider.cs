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
        void AppendNavigationState(NavigationState currentState, NavigationState state);

        void NestNavigationState(NavigationState currentState, NavigationState nestedState);

        void ReplaceNavigationState(NavigationState currentState, NavigationState state);

        bool RemoveNavigationState(NavigationState currentState);

        void UpdateCollectionState(NavigationState currentState, CollectionState collectionState);

        IView GetCurrentView(NavigationState currentState, ListUI list);

        bool TryProcessView(NavigationState currentState, IView view, bool hasSections);
    }
}

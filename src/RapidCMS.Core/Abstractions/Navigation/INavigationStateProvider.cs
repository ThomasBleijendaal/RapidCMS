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

        void ReplaceNavigationState(NavigationState state);

        void RemoveNavigationState();

        void UpdateCollectionState(CollectionState state);

        IView GetCurrentView(ListUI list);

        bool TryProcessView(IView view, bool hasSections);
    }
}

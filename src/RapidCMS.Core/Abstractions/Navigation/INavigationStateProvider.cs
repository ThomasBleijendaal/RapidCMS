using System.Threading.Tasks;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Abstractions.Navigation
{
    public interface INavigationStateProvider
    {
        NavigationState GetCurrentState();

        NavigationState Initialize(string url);

        void AppendNavigationState(NavigationState state);
    }
}

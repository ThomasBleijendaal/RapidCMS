using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface INavigationState
    {
        void PushState(NavigationStateModel newState);
        void ReplaceState(NavigationStateModel replacementState);
        NavigationStateModel? PopState();

        NavigationStateModel? GetCurrentState();
        IEnumerable<NavigationStateModel> GetCurrentStates();

        void ResetState(NavigationStateModel newState);

        IChangeToken ChangeToken { get; }
    }
}

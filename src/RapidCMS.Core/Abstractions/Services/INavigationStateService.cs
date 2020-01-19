using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using RapidCMS.Core.Models.NavigationState;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface INavigationStateService
    {
        void PushState(NavigationStateModel newState);
        void ReplaceState(NavigationStateModel replacementState);
        NavigationStateModel? PopState();

        NavigationStateModel? GetCurrentState();
        IEnumerable<NavigationStateModel> GetCurrentStates();

        void ResetState();

        IChangeToken ChangeToken { get; }
    }
}

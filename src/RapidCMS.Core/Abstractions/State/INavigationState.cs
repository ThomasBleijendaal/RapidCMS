using System;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Abstractions.State
{
    public interface INavigationState
    {
        void ResetState(PageStateModel newState);
        void NotifyLocationChanged(PageStateModel newState, bool forceReload = true);

        event EventHandler<PageStateModel>? LocationChanged;
    }
}

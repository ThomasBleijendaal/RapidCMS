using System;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Abstractions.State
{
    [Obsolete("Replace with IMediator")]
    public interface INavigationState
    {
        void ResetState(PageStateModel newState);
        void NotifyLocationChanged(PageStateModel newState);

        event EventHandler<PageStateModel>? LocationChanged;
    }
}

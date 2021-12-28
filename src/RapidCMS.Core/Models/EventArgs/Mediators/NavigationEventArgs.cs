using System;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.EventArgs.Mediators
{
    public class NavigationEventArgs : IMediatorEventArgs
    {
        public NavigationEventArgs(NavigationState? oldState, NavigationState newState)
        {
            OldState = oldState;
            NewState = newState ?? throw new ArgumentNullException(nameof(newState));
        }

        //public NavigationEventArgs(PageStateModel state, bool forceUpdate)
        //{
        //    State = state ?? throw new ArgumentNullException(nameof(state));
        //    UpdateUrl = forceUpdate;
        //}

        public NavigationState? OldState { get; set; }
        public NavigationState NewState { get; set; }
        // public bool UpdateUrl { get; set; }
    }
}

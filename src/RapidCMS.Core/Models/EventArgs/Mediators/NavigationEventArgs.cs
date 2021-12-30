using System;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.EventArgs.Mediators
{
    public class NavigationEventArgs : IMediatorEventArgs
    {
        internal NavigationEventArgs(NavigationState? oldState, NavigationState newState)
        {
            OldState = oldState;
            NewState = newState ?? throw new ArgumentNullException(nameof(newState));
        }

        public NavigationState? OldState { get; set; }
        public NavigationState NewState { get; set; }
    }
}

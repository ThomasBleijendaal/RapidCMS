using System;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Models.EventArgs.Mediators
{
    public class NavigationEventArgs : IMediatorEventArgs
    {
        public NavigationEventArgs(PageStateModel state)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            UpdateUrl = true;
        }

        public NavigationEventArgs(PageStateModel state, bool forceUpdate)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            UpdateUrl = forceUpdate;
        }

        public PageStateModel State { get; set; }
        public bool UpdateUrl { get; set; }

        ParentPath? IMediatorEventArgs.ParentPath => State.ParentPath;
    }
}

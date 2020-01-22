using System;
using RapidCMS.Core.Abstractions.Services;

namespace RapidCMS.Core.Models.State
{
    public class ViewState
    {
        public ViewState(INavigationState navigationState)
        {
            NavigationState = navigationState ?? throw new ArgumentNullException(nameof(navigationState));
        }

        public INavigationState NavigationState { get; private set; }
    }
}

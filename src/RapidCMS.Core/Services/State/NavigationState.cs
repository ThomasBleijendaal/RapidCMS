using System;
using Microsoft.JSInterop;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Services.State
{
    internal class NavigationState : INavigationState
    {
        private readonly IJSRuntime _jsRuntime;

        public NavigationState(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public void ResetState(PageStateModel newState)
        {
            NotifyLocationChanged(newState);
        }

        public void NotifyLocationChanged(PageStateModel newState)
        {
            LocationChanged?.Invoke(default, newState);

            var parentPath = newState.ParentPath?.ToPathString();

            _jsRuntime.InvokeVoidAsync("RapidCMS.navigateTo", (newState.PageType == PageType.Collection) 
                ? $"/collection/{GetAction(newState.UsageType)}{(parentPath == null ? "" : $"/{parentPath}")}/{newState.CollectionAlias}"
                : $"/node/{GetAction(newState.UsageType)}{(parentPath == null ? "" : $"/{parentPath}")}/{newState.CollectionAlias}/{newState.VariantAlias}/{newState.Id}");
        }

        private string GetAction(UsageType usageType)
        {
            return usageType switch
            {
                _ when usageType.HasFlag(UsageType.Edit) => Constants.Edit,
                _ when usageType.HasFlag(UsageType.View) => Constants.View,
                _ => "unknown"
            };
        }

        public event EventHandler<PageStateModel>? LocationChanged;
    }
}

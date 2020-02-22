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

        public void NotifyLocationChanged(PageStateModel newState, bool forceReload = true)
        {
            if (forceReload)
            {
                LocationChanged?.Invoke(default, newState);
            }

            var parentPath = newState.ParentPath?.ToPathString();

            var url = (newState.PageType == PageType.Collection) ? $"/collection/{GetAction(newState.UsageType)}{(parentPath == null ? "" : $"/{parentPath}")}/{newState.CollectionAlias}"
                : (newState.PageType == PageType.Node) ? $"/node/{GetAction(newState.UsageType)}{(parentPath == null ? "" : $"/{parentPath}")}/{newState.CollectionAlias}/{newState.VariantAlias}/{newState.Id}"
                : (newState.PageType == PageType.Page) ? $"/page/{newState.CollectionAlias}" : "/";

            _jsRuntime.InvokeVoidAsync("RapidCMS.navigateTo", url);
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

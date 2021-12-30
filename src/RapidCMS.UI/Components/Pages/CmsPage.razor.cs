using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Models.EventArgs.Mediators;

namespace RapidCMS.UI.Components.Pages
{
    [Authorize]
    public partial class CmsPage
    {
        // protected PageStateModel? State { get; set; }

        [Inject] private INavigationStateProvider NavigationState { get; set; } = default!;

        [Inject] private IMediator Mediator { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public string? PageRoute { get; set; }

        //[Parameter] public string Action { get; set; } = default!;
        //[Parameter] public string CollectionAlias { get; set; } = default!;
        //[Parameter] public string VariantAlias { get; set; } = default!;
        //[Parameter] public string? Path { get; set; } = default!;
        //[Parameter] public string? Id { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //DisposeWhenDisposing(Mediator.RegisterCallback<NavigationEventArgs>(LocationChangedAsync));

            NavigationState.Initialize(PageRoute ?? "", new Uri(NavigationManager.Uri).Query);
        }

        protected override void OnParametersSet()
        {

            //State = null;

            //var pageType = GetPageType();

            //if (pageType == PageType.Dashboard)
            //{
            //    State = new PageStateModel
            //    {
            //        PageType = PageType.Dashboard
            //    };
            //}
            //else
            //{
            //    State = new PageStateModel
            //    {
            //        PageType = pageType,
            //        UsageType = GetUsageType(),
            //        CollectionAlias = CollectionAlias,
            //        Id = Id,
            //        ParentPath = ParentPath.TryParse(Path),
            //        VariantAlias = VariantAlias
            //    };
            //}

            //Mediator.NotifyEvent(this, new NavigationEventArgs(State, false));
        }

        private async Task LocationChangedAsync(object sender, NavigationEventArgs args)
        {
            //StateHasChanged();

            //if (args.UpdateUrl)
            //{
            //    await InvokeAsync(async () =>
            //    {
            //        State = args.State;

            //        var parentPath = State.ParentPath?.ToPathString();

            //        var url = (State.PageType == PageType.Collection) ? $"/collection/{GetAction(State.UsageType)}{(string.IsNullOrEmpty(parentPath) ? "" : $"/{parentPath}")}/{State.CollectionAlias}"
            //            : (State.PageType == PageType.Node) ? $"/node/{GetAction(State.UsageType)}{(string.IsNullOrEmpty(parentPath) ? "" : $"/{parentPath}")}/{State.CollectionAlias}/{State.VariantAlias}/{State.Id}"
            //            : (State.PageType == PageType.Page) ? $"/page/{State.CollectionAlias}" : "/";

            //        await JSRuntime.InvokeVoidAsync("RapidCMS.navigateTo", url);

            //        StateHasChanged();
            //    });
            //}
        }

        //protected override void Dispose()
        //{
        //    State = null;
        //}

        //private UsageType GetUsageType()
        //{
        //    var type = Action switch
        //    {
        //        Constants.Edit => UsageType.Edit,
        //        Constants.New => UsageType.New,
        //        Constants.Add => UsageType.Add,
        //        Constants.View => UsageType.View | ((GetPageType() == PageType.Collection) ? UsageType.List : UsageType.Node),
        //        Constants.Pick => UsageType.Pick,
        //        _ => (UsageType)0
        //    };

        //    if (Path == null)
        //    {
        //        type |= UsageType.Root;
        //    }
        //    else
        //    {
        //        type |= UsageType.NotRoot;
        //    }

        //    return type;
        //}

        //private PageType GetPageType()
        //{
        //    return NavigationManager.Uri.Contains("/collection/") ? PageType.Collection :
        //           NavigationManager.Uri.Contains("/node/") ? PageType.Node :
        //           NavigationManager.Uri.Contains("/page/") ? PageType.Page :
        //           PageType.Dashboard;
        //}

        //private static string GetAction(UsageType usageType)
        //{
        //    return usageType switch
        //    {
        //        _ when usageType.HasFlag(UsageType.Edit) => Constants.Edit,
        //        _ when usageType.HasFlag(UsageType.View) => Constants.View,
        //        _ when usageType.HasFlag(UsageType.New) => Constants.New,
        //        _ when usageType.HasFlag(UsageType.Add) => Constants.Add,
        //        _ when usageType.HasFlag(UsageType.Pick) => Constants.Pick,
        //        _ => "unknown"
        //    };
        //}
    }
}

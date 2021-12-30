using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Navigation;

namespace RapidCMS.UI.Components.Pages
{
    [Authorize]
    public partial class CmsPage
    {
        [Inject] private INavigationStateProvider NavigationState { get; set; } = default!;

        [Inject] private IMediator Mediator { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        
        [Parameter] public string? PageRoute { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigationState.Initialize(PageRoute ?? "", new Uri(NavigationManager.Uri).Query);
        }
    }
}

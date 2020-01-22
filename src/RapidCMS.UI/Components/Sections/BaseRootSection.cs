using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection : ComponentBase
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IExceptionService ExceptionService { get; set; } = default!;
        [Inject] protected INavigationState NavigationState { get; set; } = default!;

        [Inject] protected IPresentationService PresentationService { get; set; } = default!;
        [Inject] protected IInteractionService InteractionService { get; set; } = default!;
        [Inject] protected IUIResolverFactory UIResolverFactory { get; set; } = default!;

        [Parameter] public NavigationStateModel InitialState { get; set; } = default!;
        protected NavigationStateModel CurrentState => NavigationState.GetCurrentState()!;

        protected IEnumerable<ButtonUI>? Buttons { get; set; }
        protected IEnumerable<(EditContext editContext, IEnumerable<SectionUI> sections)>? Sections { get; set; }

        protected ViewState CurrentViewState => new ViewState(NavigationState);

        protected async Task HandleViewCommandAsync(ViewCommandResponseModel response)
        {
            try
            {
                if (response.NoOp)
                {
                    return;
                }

                await LoadDataAsync(response.RefreshIds);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                NavigationState.ResetState(InitialState);

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private Task LoadDataAsync(IEnumerable<string>? entityIds = null)
        {
            if (CurrentState?.PageType == PageType.Node)
            {
                return LoadNodeDataAsync();
            }
            else if (CurrentState?.PageType == PageType.Collection)
            {
                return LoadCollectionDataAsync(entityIds);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        protected void HandleException(Exception ex)
        {
            // meh
            if (ex is UnauthorizedAccessException)
            {
                NavigationManager.NavigateTo("/unauthorized");
            }
            else if (ex is InvalidEntityException)
            {
                // trigger validation since entity is invalid
                // EditContext.IsValid();
            }
            else
            {
                ExceptionService.StoreException(ex);

                NavigationManager.NavigateTo("/error");
            }
        }
    }
}

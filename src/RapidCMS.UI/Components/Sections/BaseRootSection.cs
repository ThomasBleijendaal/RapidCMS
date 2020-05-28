using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection : ComponentBase
    {
        [Inject] private IExceptionService ExceptionService { get; set; } = default!;
        [Inject] protected IPageState PageState { get; set; } = default!;
        [Inject] protected IMessageService MessageService { get; set; } = default!;

        [Inject] protected IPresentationService PresentationService { get; set; } = default!;
        [Inject] protected IInteractionService InteractionService { get; set; } = default!;
        [Inject] protected IUIResolverFactory UIResolverFactory { get; set; } = default!;

        [Parameter] public bool IsRoot { get; set; }
        [Parameter] public PageStateModel InitialState { get; set; } = default!;
        protected PageStateModel CurrentState => PageState.GetCurrentState()!;

        protected IEnumerable<ButtonUI>? Buttons { get; set; }
        protected IEnumerable<(EditContext editContext, IEnumerable<SectionUI> sections)>? Sections { get; set; }
        protected IEnumerable<ITypeRegistration>? PageContents { get; set; }

        protected ViewState CurrentViewState => new ViewState(PageState);

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
                Buttons = null;
                Sections = null;
                ListContext = null;
                Tabs = null;
                UIResolver = null;
                ListUI = null;
                EditContext = null;
                PageContents = null;

                PageState.ResetState(InitialState);

                if (IsRoot)
                {
                    PageState.UpdateNavigationStateWhenStateChanges();
                }

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async Task LoadDataAsync(IEnumerable<string>? entityIds = null)
        {
            // TODO: investigate whether its good to clear the Buttons, Sections and PageContents first
            // there is a tiny timing difference when on WebAssembly and a node view to list view shows
            // the node view in the list view for a short period of time

            if (CurrentState?.PageType == PageType.Node)
            {
                await LoadNodeDataAsync();
            }
            else if (CurrentState?.PageType == PageType.Collection)
            {
                await LoadCollectionDataAsync(entityIds);
            }
            else if (CurrentState?.PageType == PageType.Page)
            {
                await LoadPageDataAsync();
            }
            else if (CurrentState?.PageType == PageType.Dashboard)
            {
                await LoadPageDataAsync();
            }
        }

        protected void HandleException(Exception ex)
        {
            if (ex is UnauthorizedAccessException)
            {
                PageState.ResetState(new PageStateModel { PageType = PageType.Unauthorized });
            }
            else if (ex is InvalidEntityException)
            {
                MessageService.AddMessage(MessageType.Error, "Failed to perform action, Entity is in invalid state.");
            }
            else
            {
                ExceptionService.StoreException(ex);

                PageState.ResetState(new PageStateModel { PageType = PageType.Error });
            }
        }

        protected RenderFragment RenderType(ITypeRegistration section)
        {
            return builder =>
            {
                var type = section.Type == typeof(ICollectionConfig)
                    ? typeof(RootSection)
                    : section.Type;

                builder.OpenComponent(0, type);

                if (section.Parameters != null)
                {
                    var index = 1;
                    section.Parameters.ForEach(kv =>
                    {
                        builder.AddAttribute(index++, kv.Key, kv.Value);
                    });
                }

                builder.CloseComponent();
            };
        }
    }
}

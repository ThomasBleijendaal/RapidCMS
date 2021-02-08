using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection : DisposableComponent
    {
        [Inject] protected ICms Cms { get; set; } = default!;
        [Inject] protected IPageState PageState { get; set; } = default!;
        [Inject] protected IMediator Mediator { get; set; } = default!;

        [Inject] protected IPresentationService PresentationService { get; set; } = default!;
        [Inject] protected IInteractionService InteractionService { get; set; } = default!;
        [Inject] protected IUIResolverFactory UIResolverFactory { get; set; } = default!;

        [Parameter] public bool IsRoot { get; set; }
        [Parameter] public PageStateModel InitialState { get; set; } = default!;

        protected bool StateIsChanging { get; set; } = false;

        protected PageStateModel CurrentState => PageState.GetCurrentState()!;

        protected IEnumerable<ButtonUI>? Buttons { get; set; }
        protected IEnumerable<(FormEditContext editContext, IEnumerable<SectionUI> sections)>? Sections { get; set; }
        protected IEnumerable<ITypeRegistration>? PageContents { get; set; }

        protected ViewState CurrentViewState => new ViewState(PageState);

        protected async Task HandleViewCommandAsync(ViewCommandResponseModel response)
        {
            try
            {
                StateIsChanging = true;

                if (response.NoOp)
                {
                    return;
                }

                await LoadDataAsync(response.RefreshIds);
            }
            catch (Exception ex)
            {
                Mediator.NotifyEvent(this, new ExceptionEventArgs(ex));
            }
            finally
            {
                StateIsChanging = false;
                StateHasChanged();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            DisposeWhenDisposing(Mediator.RegisterCallback<CollectionRepositoryEventArgs>(OnRepositoryActionAsync));
            DisposeWhenDisposing(Mediator.RegisterCallback<ExceptionEventArgs>(OnExceptionAsync));
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
                Mediator.NotifyEvent(this, new ExceptionEventArgs(ex));
            }
        }

        private async Task LoadDataAsync(IEnumerable<string>? entityIds = null)
        {   
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

        private async Task OnRepositoryActionAsync(object sender, CollectionRepositoryEventArgs args)
        {
            if (CurrentState == null || 
                CurrentState.CollectionAlias == null || 
                !CurrentState.UsageType.HasFlag(UsageType.View) || 
                args.CollectionAlias != CurrentState.CollectionAlias ||
                StateIsChanging)
            {
                return;
            }

            await InvokeAsync(async () => await LoadDataAsync());
        }

        private async Task OnExceptionAsync(object sender, ExceptionEventArgs args)
        {
            await InvokeAsync(() =>
            {
                if (args.Exception is UnauthorizedAccessException)
                {
                    PageState.ResetState(new PageStateModel { PageType = PageType.Unauthorized });
                }
                else if (args.Exception is InvalidEntityException)
                {
                    Mediator.NotifyEvent(this, new MessageEventArgs(MessageType.Error, "Failed to perform action, Entity is in invalid state."));
                }
                else if (!Cms.IsDevelopment)
                {
                    Mediator.NotifyEvent(this, new MessageEventArgs(MessageType.Error, $"Failed to perform action: {args.Exception.Message}."));
                }
                else
                { 
                    PageState.ResetState(new PageStateModel { PageType = PageType.Error });
                }
            });
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

        protected override bool ShouldRender()
        {
            return !StateIsChanging;
        }
    }
}

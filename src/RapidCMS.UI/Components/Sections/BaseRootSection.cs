using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Navigation;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection : DisposableComponent
    {
        [Inject] protected ICms Cms { get; set; } = default!;
        [Inject] protected IMediator Mediator { get; set; } = default!;
        [Inject] private INavigationStateProvider NavigationStateProvider { get; set; } = default!;

        [Obsolete("Find out how to remove this")]
        [Inject] protected IServiceProvider ServiceProvider { get; set; } = default!;
        [Inject] protected IPresentationService PresentationService { get; set; } = default!;
        [Inject] protected IInteractionService InteractionService { get; set; } = default!;
        [Inject] protected IUIResolverFactory UIResolverFactory { get; set; } = default!;

        protected int Update { get; set; } = 0;

        protected bool StateIsChanging { get; set; } = false;

        protected NavigationState CurrentNavigationState { get; private set; } = default!;

        [Parameter] public NavigationState? InitialState { get; set; }

        protected IEnumerable<ButtonUI>? Buttons { get; set; }
        protected List<(FormEditContext editContext, IEnumerable<SectionUI> sections)>? Sections { get; set; }
        protected (PageContext? pageContext, IEnumerable<TypeRegistrationSetup>? sections) PageContents { get; set; }

        private CancellationTokenSource _loadCancellationTokenSource = new CancellationTokenSource();

        protected async Task HandleViewCommandAsync<TViewCommandResponseModel>(Func<Task<TViewCommandResponseModel>> viewCommand)
            where TViewCommandResponseModel : ViewCommandResponseModel
        {
            try
            {
                StateIsChanging = true;

                var response = await viewCommand.Invoke();

                if (response.NoOp)
                {
                    return;
                }
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
            DisposeWhenDisposing(Mediator.RegisterCallback<NavigationEventArgs>(OnNavigationAsync));
            
            CurrentNavigationState = InitialState ?? NavigationStateProvider.GetCurrentState();

            await LoadDataAsync();
        }

        protected async Task OnNavigationAsync(object sender, NavigationEventArgs args)
        {
            if ((InitialState == null && args.OldState == null) || CurrentNavigationState.Equals(args.OldState))
            {
                CurrentNavigationState = args.NewState;

                await LoadDataAsync();

                StateHasChanged();
            }
        }

        private async Task LoadDataAsync(IEnumerable<string>? entityIds = null)
        {
            Update++;

            CancelLoadAndRestart();

            if (CurrentNavigationState.PageType == PageType.Node)
            {
                await LoadNodeDataAsync(_loadCancellationTokenSource.Token);
            }
            else if (CurrentNavigationState.PageType == PageType.Collection)
            {
                await LoadCollectionDataAsync(_loadCancellationTokenSource.Token, entityIds);
            }
            else if (CurrentNavigationState.PageType == PageType.Page)
            {
                await LoadPageDataAsync(_loadCancellationTokenSource.Token);
            }
            else if (CurrentNavigationState.PageType == PageType.Dashboard)
            {
                await LoadPageDataAsync(_loadCancellationTokenSource.Token);
            }
        }

        private async Task OnRepositoryActionAsync(object sender, CollectionRepositoryEventArgs args)
        {
            if (!CurrentNavigationState.HasCollectionAlias || args.CollectionAlias != CurrentNavigationState.CollectionAlias)
            {
                return;
            }

            await InvokeAsync(() => LoadDataAsync());
        }

        private async Task OnExceptionAsync(object sender, ExceptionEventArgs args)
        {
            await InvokeAsync(() =>
            {
                if (args.Exception is UnauthorizedAccessException)
                {
                    NavigationStateProvider.ReplaceNavigationState(CurrentNavigationState, new NavigationState(PageType.Unauthorized));
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
                    NavigationStateProvider.ReplaceNavigationState(CurrentNavigationState, new NavigationState(PageType.Error));
                }
            });
        }

        protected static RenderFragment RenderType(TypeRegistrationSetup section)
            => builder =>
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

        protected override bool ShouldRender()
        {
            return !StateIsChanging;
        }

        private void CancelLoadAndRestart()
        {
            var currentSource = _loadCancellationTokenSource;
            _loadCancellationTokenSource = new CancellationTokenSource();
            currentSource.Cancel();
        }
    }
}

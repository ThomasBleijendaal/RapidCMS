using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.EventArgs;
using RapidCMS.Core.Models.NavigationState;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Sections
{
    public abstract class BaseRootSection : ComponentBase
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IExceptionService ExceptionService { get; set; } = default!;
        [Inject] protected INavigationStateService NavigationState { get; set; } = default!;

        [Inject] protected IPresentationService PresentationService { get; set; } = default!;
        [Inject] protected IInteractionService InteractionService { get; set; } = default!;
        [Inject] protected IUIResolverFactory UIResolverFactory { get; set; } = default!;

        [Parameter] public NavigationStateModel InitialState { get; set; } = default!;
        protected NavigationStateModel CurrentState => NavigationState.GetCurrentState()!;

        protected async Task HandleViewCommandAsync(ViewCommandResponseModel response)
        {
            try
            {
                // TODO: add support for view state change via change token
                //if (response.ViewCommand != null && (response.ViewCommand.RefreshIds.Any() || response.ViewCommand.ReloadData))
                //{
                await LoadDataAsync(response.ViewCommand?.RefreshIds);
                //}
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
                NavigationState.ResetState();
                NavigationState.PushState(InitialState);

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

        // combined stuff
        protected IEnumerable<ButtonUI>? Buttons { get; set; }

        protected IEnumerable<(EditContext editContext, IEnumerable<SectionUI> sections)>? Sections { get; set; }

        // node stuff
        protected EditContext? EditContext { get; set; }

        protected async Task LoadNodeDataAsync()
        {
            try
            {
                if (CurrentState == null)
                {
                    throw new InvalidOperationException();
                }

                var editContext = await PresentationService.GetEntityAsync(new GetEntityRequestModel
                {
                    CollectionAlias = CurrentState.CollectionAlias,
                    Id = CurrentState.Id,
                    ParentPath = CurrentState.ParentPath,
                    UsageType = CurrentState.UsageType,
                    VariantAlias = CurrentState.VariantAlias
                });

                var resolver = await UIResolverFactory.GetNodeUIResolverAsync(CurrentState.UsageType, CurrentState.CollectionAlias);

                Buttons = await resolver.GetButtonsForEditContextAsync(editContext);
                Sections = new[] { (editContext, await resolver.GetSectionsForEditContextAsync(editContext)) };

                EditContext = editContext;

                EditContext.OnFieldChanged += (s, a) => StateHasChanged();

                StateHasChanged();
            }
            catch
            {
                EditContext = null;

                throw;
            }
        }

        protected async Task ButtonOnClickAsync(ButtonClickEventArgs args)
        {
            try
            {
                if (args.ViewModel == null)
                {
                    throw new ArgumentException($"ViewModel required");
                }

                var command = await InteractionService.InteractAsync<PersistEntityRequestModel, NodeViewCommandResponseModel>(new PersistEntityRequestModel
                {
                    ActionId = args.ViewModel.ButtonId,
                    CustomData = args.Data,
                    EditContext = args.EditContext
                }, NavigationState);

                await HandleViewCommandAsync(command);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        // collection stuff

        protected ListContext? ListContext { get; set; }

        protected IEnumerable<TabUI>? Tabs { get; set; }

        protected IListUIResolver? UIResolver { get; set; }
        protected ListUI? ListUI { get; set; }

        protected async Task LoadCollectionDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            try
            {
                if (CurrentState == null)
                {
                    throw new InvalidOperationException();
                }

                if (reloadEntityIds?.Any() == true)
                {
                    await ReloadSectionsAsync(reloadEntityIds);
                }
                else
                {
                    UIResolver = await UIResolverFactory.GetListUIResolverAsync(CurrentState.UsageType, CurrentState.CollectionAlias);
                    ListUI = UIResolver.GetListDetails();

                    var listContext = await LoadSectionsAsync();

                    Buttons = await UIResolver.GetButtonsForEditContextAsync(listContext.ProtoEditContext);
                    Tabs = await UIResolver.GetTabsAsync(listContext.ProtoEditContext);

                    ListContext = listContext;
                    Sections?.ForEach(x => x.editContext.OnFieldChanged += (s, a) => StateHasChanged());
                }

                StateHasChanged();
            }
            catch
            {
                ListContext = null;
                Sections = null;
                ListUI = null;

                throw;
            }
        }

        protected async Task PageChangedAsync(int page)
        {
            CurrentState.CurrentPage = page;

            await LoadSectionsAsync();
            StateHasChanged();
        }

        protected async Task SearchAsync(string? search)
        {
            CurrentState.CurrentPage = 1;
            CurrentState.SearchTerm = search;

            await LoadSectionsAsync();
            StateHasChanged();
        }

        protected async Task TabChangeAsync(int? tabId)
        {
            CurrentState.ActiveTab = tabId;
            CurrentState.CurrentPage = 1;

            await LoadSectionsAsync();
            StateHasChanged();
        }

        protected async Task<ListContext> LoadSectionsAsync()
        {
            var query = Query.Create(ListUI!.PageSize, CurrentState.CurrentPage, CurrentState.SearchTerm, CurrentState.ActiveTab);

            if (ListUI.OrderBys != null)
            {
                query.SetOrderByExpressions(ListUI.OrderBys);
            }

            var listContext = await PresentationService.GetEntitiesAsync(new GetEntitiesOfParentRequestModel
            {
                CollectionAlias = CurrentState.CollectionAlias,
                ParentPath = CurrentState.ParentPath,
                Query = query,
                UsageType = CurrentState.UsageType
            });

            await SetSectionsAsync(listContext);

            if (!query.MoreDataAvailable)
            {
                CurrentState.MaxPage = CurrentState.CurrentPage;

                if (CurrentState.CurrentPage > 1 && !Sections.Any())
                {
                    CurrentState.CurrentPage--;
                    CurrentState.MaxPage = null;
                    await LoadSectionsAsync();
                }
            }
            if (CurrentState.MaxPage == CurrentState.CurrentPage && query.MoreDataAvailable)
            {
                CurrentState.MaxPage = null;
            }

            return listContext;
        }

        private async Task SetSectionsAsync(ListContext listContext)
        {
            Sections = await listContext.EditContexts.ToListAsync(async editContext => (editContext, await UIResolver.GetSectionsForEditContextAsync(editContext)));
        }

        protected async Task ReloadSectionsAsync(IEnumerable<string> reloadEntityIds)
        {
            if (UIResolver == null || Sections == null)
            {
                return;
            }

            var newSections = await Sections.ToListAsync(async x =>
            {
                if (reloadEntityIds.Contains(x.editContext.Entity.Id))
                {
                    var reloadedEditContext = await PresentationService.GetEntityAsync(new GetEntityRequestModel
                    {
                        CollectionAlias = x.editContext.CollectionAlias,
                        Id = x.editContext.Entity.Id,
                        ParentPath = x.editContext.Parent?.GetParentPath(),
                        UsageType = x.editContext.UsageType
                    });

                    return (reloadedEditContext, await UIResolver.GetSectionsForEditContextAsync(reloadedEditContext));
                }
                else
                {
                    return x;
                }
            });

            Sections = newSections;
        }

        protected async Task ListButtonOnClickAsync(ButtonClickEventArgs args)
        {
            try
            {
                if (CurrentState == null)
                {
                    throw new InvalidOperationException();
                }

                var request = new PersistEntitiesRequestModel
                {
                    ActionId = args.ViewModel.ButtonId,
                    CustomData = args.Data,
                    ListContext = ListContext!
                };

                var command = CurrentState.UsageType.HasFlag(UsageType.Edit)
                    ? (ViewCommandResponseModel)await InteractionService.InteractAsync<PersistEntitiesRequestModel, ListEditorCommandResponseModel>(request, NavigationState)
                    : (ViewCommandResponseModel)await InteractionService.InteractAsync<PersistEntitiesRequestModel, ListViewCommandResponseModel>(request, NavigationState);

                await HandleViewCommandAsync(command);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected async Task NodeButtonOnClickAsync(ButtonClickEventArgs args)
        {
            try
            {
                var command = await InteractionService.InteractAsync<PersistEntityRequestModel, NodeInListViewCommandResponseModel>(new PersistEntityRequestModel
                {
                    ActionId = args.ViewModel.ButtonId,
                    CustomData = args.Data,
                    EditContext = args.EditContext
                }, NavigationState);

                await HandleViewCommandAsync(command);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected async Task OnRowDraggedAsync(RowDragEventArgs args)
        {
            try
            {
                if (ListContext == null)
                {
                    throw new InvalidOperationException();
                }

                var editContext = ListContext.EditContexts.FirstOrDefault(x => x.Entity.Id == args.SubjectId);
                if (editContext == null)
                {
                    throw new InvalidOperationException();
                }

                editContext.NotifyReordered(args.TargetId);

                ListContext.EditContexts.Remove(editContext);

                if (args.TargetId == null)
                {
                    ListContext.EditContexts.Add(editContext);
                }
                else
                {
                    ListContext.EditContexts.Insert(
                        ListContext.EditContexts.FindIndex(x => x.Entity.Id == args.TargetId),
                        editContext);
                }

                await SetSectionsAsync(ListContext);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

    }
}

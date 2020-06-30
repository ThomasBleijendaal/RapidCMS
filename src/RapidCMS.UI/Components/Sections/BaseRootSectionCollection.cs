using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.EventArgs;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection
    {
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
                query.SetOrderBys(ListUI.OrderBys);
            }

            var request = CurrentState.Related != null
                ? (GetEntitiesRequestModel)new GetEntitiesOfRelationRequestModel
                {
                    CollectionAlias = CurrentState.CollectionAlias,
                    Query = query,
                    Related = CurrentState.Related,
                    UsageType = CurrentState.UsageType
                }
                : (GetEntitiesRequestModel)new GetEntitiesOfParentRequestModel
                {
                    CollectionAlias = CurrentState.CollectionAlias,
                    ParentPath = CurrentState.ParentPath,
                    Query = query,
                    UsageType = CurrentState.UsageType
                };

            var listContext = await PresentationService.GetEntitiesAsync<GetEntitiesRequestModel, ListContext>(request);

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
            if (UIResolver == null)
            {
                return;
            }

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
                if (reloadEntityIds.Contains<string>(x.editContext.Entity.Id!))
                {
                    var reloadedEditContext = await PresentationService.GetEntityAsync<GetEntityRequestModel, EditContext>(new GetEntityRequestModel
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
                    ListContext = ListContext!,
                    Related = CurrentState.Related
                };

                var command = CurrentState.UsageType.HasFlag(UsageType.Edit)
                    ? (ViewCommandResponseModel)await InteractionService.InteractAsync<PersistEntitiesRequestModel, ListEditorCommandResponseModel>(request, CurrentViewState)
                    : (ViewCommandResponseModel)await InteractionService.InteractAsync<PersistEntitiesRequestModel, ListViewCommandResponseModel>(request, CurrentViewState);

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
                var command = (CurrentState.Related != null)
                    ? await InteractionService.InteractAsync<PersistRelatedEntityRequestModel, NodeInListViewCommandResponseModel>(new PersistRelatedEntityRequestModel
                    {
                        ActionId = args.ViewModel.ButtonId,
                        CustomData = args.Data,
                        EditContext = args.EditContext,
                        Related = CurrentState.Related
                    }, CurrentViewState)
                    : await InteractionService.InteractAsync<PersistEntityRequestModel, NodeInListViewCommandResponseModel>(new PersistEntityRequestModel
                    {
                        ActionId = args.ViewModel.ButtonId,
                        CustomData = args.Data,
                        EditContext = args.EditContext,
                    }, CurrentViewState);

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

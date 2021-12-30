using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Navigation;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Sections
{
    public abstract partial class BaseRootSection
    {
        protected ListContext? ListContext { get; set; }

        protected IEnumerable<TabUI>? Tabs { get; set; }

        protected ListUI? ListUI { get; set; }

        private CollectionState CollectionState => CurrentNavigationState.CollectionState;

        protected async Task LoadCollectionDataAsync(CancellationToken cancellationToken, IEnumerable<string>? reloadEntityIds = null)
        {
            var uiResolver = await UIResolverFactory.GetListUIResolverAsync(CurrentNavigationState);

            if (reloadEntityIds?.Any() == true)
            {
                var sections = await ReloadSectionsAsync(reloadEntityIds, uiResolver);

                if (cancellationToken.IsCancellationRequested || ListContext == null)
                {
                    return;
                }

                ListContext.EditContexts.Clear();
                ListContext.EditContexts.AddRange(sections.Select(x => x.editContext));
                Sections = sections;
            }
            else
            {
                var listUI = uiResolver.GetListDetails();

                var (listContext, sections) = await LoadSectionsAsync(listUI, uiResolver);

                var buttons = await uiResolver.GetButtonsForEditContextAsync(listContext.ProtoEditContext);
                var tabs = await uiResolver.GetTabsAsync(listContext.ProtoEditContext);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    ListUI = listUI;
                    Buttons = buttons;
                    Tabs = tabs;
                    ListContext = listContext;
                    Sections = sections;

                    PageContents = null;
                }
                catch
                {
                }
            }

            StateHasChanged();
        }

        protected void PageChanged(int page)
        {
            if (ListUI == null)
            {
                return;
            }

            NavigationStateProvider.UpdateCollectionState(CurrentNavigationState, CollectionState with
            {
                CurrentPage = page
            });
        }

        protected void Search(string? search)
        {
            if (ListUI == null)
            {
                return;
            }

            NavigationStateProvider.UpdateCollectionState(CurrentNavigationState, CollectionState with
            {
                CurrentPage = 1,
                SearchTerm = search
            });
        }

        protected void TabChange(int? tabId)
        {
            if (ListUI == null)
            {
                return;
            }

            NavigationStateProvider.UpdateCollectionState(CurrentNavigationState, CollectionState with
            {
                ActiveTab = tabId,
                CurrentPage = 1
            });
        }

        protected async Task<(ListContext listContext, List<(FormEditContext editContext, IEnumerable<SectionUI> sections)> sections)> LoadSectionsAsync(ListUI listUI, IListUIResolver uiResolver)
        {
            var view = NavigationStateProvider.GetCurrentView(CurrentNavigationState, listUI);

            var request = CurrentNavigationState.Related != null
                ? (GetEntitiesRequestModel)new GetEntitiesOfRelationRequestModel
                {
                    CollectionAlias = CurrentNavigationState.CollectionAlias,
                    View = view,
                    Related = CurrentNavigationState.Related,
                    UsageType = CurrentNavigationState.UsageType,
                    VariantAlias = CurrentNavigationState.VariantAlias,
                    IsEmbedded = !NavigationStateProvider.IsRootState(CurrentNavigationState)
                }
                : (GetEntitiesRequestModel)new GetEntitiesOfParentRequestModel
                {
                    CollectionAlias = CurrentNavigationState.CollectionAlias,
                    ParentPath = CurrentNavigationState.ParentPath,
                    View = view,
                    UsageType = CurrentNavigationState.UsageType,
                    VariantAlias = CurrentNavigationState.VariantAlias,
                    IsEmbedded = !NavigationStateProvider.IsRootState(CurrentNavigationState)
                };

            var listContext = await PresentationService.GetEntitiesAsync<GetEntitiesRequestModel, ListContext>(request);

            var sections = await listContext.EditContexts.ToListAsync(async editContext => (editContext, await uiResolver.GetSectionsForEditContextAsync(editContext, CurrentNavigationState)));

            if (!NavigationStateProvider.TryProcessView(CurrentNavigationState, view, sections.Any() == true))
            {
                return await LoadSectionsAsync(listUI, uiResolver);
            }

            return (listContext, sections);
        }

        protected async Task<List<(FormEditContext editContext, IEnumerable<SectionUI> sections)>> ReloadSectionsAsync(IEnumerable<string> reloadEntityIds, IListUIResolver uiResolver)
        {
            if (Sections == null)
            {
                return new List<(FormEditContext editContext, IEnumerable<SectionUI> sections)>();
            }

            var newSections = await Sections.ToListAsync(async x =>
            {
                if (reloadEntityIds.Contains<string>(x.editContext.Entity.Id!))
                {
                    var reloadedEditContext = await PresentationService.GetEntityAsync<GetEntityRequestModel, FormEditContext>(new GetEntityRequestModel
                    {
                        CollectionAlias = x.editContext.CollectionAlias,
                        Id = x.editContext.Entity.Id,
                        ParentPath = x.editContext.Parent?.GetParentPath(),
                        UsageType = x.editContext.UsageType,
                        VariantAlias = x.editContext.EntityVariantAlias
                    });

                    return (reloadedEditContext, await uiResolver.GetSectionsForEditContextAsync(reloadedEditContext, CurrentNavigationState));
                }
                else
                {
                    return x;
                }
            });

            return newSections;
        }

        protected async Task ListButtonOnClickAsync(ButtonClickEventArgs args)
        {
            try
            {
                if (CurrentNavigationState == null)
                {
                    throw new InvalidOperationException();
                }

                var request = new PersistEntitiesRequestModel
                {
                    ActionId = args.ViewModel.ButtonId,
                    CustomData = args.Data,
                    ListContext = ListContext!,
                    Related = CurrentNavigationState.Related,
                    NavigationState = CurrentNavigationState
                };

                if (CurrentNavigationState.UsageType.HasFlag(UsageType.Edit))
                {
                    await HandleViewCommandAsync(() => InteractionService.InteractAsync<PersistEntitiesRequestModel, ListEditorCommandResponseModel>(request));
                }
                else
                {
                    await HandleViewCommandAsync(() => InteractionService.InteractAsync<PersistEntitiesRequestModel, ListViewCommandResponseModel>(request));
                }
            }
            catch (Exception ex)
            {
                Mediator.NotifyEvent(this, new ExceptionEventArgs(ex));
            }
        }

        protected async Task NodeButtonOnClickAsync(ButtonClickEventArgs args)
        {
            try
            {
                if (CurrentNavigationState.Related != null)
                {
                    await HandleViewCommandAsync(() => InteractionService.InteractAsync<PersistRelatedEntityRequestModel, NodeInListViewCommandResponseModel>(new PersistRelatedEntityRequestModel
                    {
                        ActionId = args.ViewModel.ButtonId,
                        CustomData = args.Data,
                        EditContext = args.EditContext,
                        Related = CurrentNavigationState.Related,
                        NavigationState = CurrentNavigationState
                    }));
                }
                else
                {
                    await HandleViewCommandAsync(() => InteractionService.InteractAsync<PersistEntityRequestModel, NodeInListViewCommandResponseModel>(new PersistEntityRequestModel
                    {
                        ActionId = args.ViewModel.ButtonId,
                        CustomData = args.Data,
                        EditContext = args.EditContext,
                        NavigationState = CurrentNavigationState
                    }));
                }
            }
            catch (Exception ex)
            {
                Mediator.NotifyEvent(this, new ExceptionEventArgs(ex));
            }
        }

        protected void OnRowDragged(RowDragEventArgs args)
        {
            try
            {
                if (ListContext == null || Sections == null)
                {
                    throw new InvalidOperationException();
                }

                var beforeIds = ListContext.EditContexts.Select(x => x.Entity.Id).ToArray();
                var beforeSections = Sections.Select(x => x.editContext.Entity.Id).ToArray();

                var subjectIndex = ListContext.EditContexts.FindIndex(x => x.Entity.Id == args.SubjectId);
                var targetIndex = args.TargetId == null ? -1 : ListContext.EditContexts.FindIndex(x => x.Entity.Id == args.TargetId);
                if (targetIndex > subjectIndex)
                {
                    targetIndex--;
                }

                var editContext = ListContext.EditContexts.ElementAt(subjectIndex);
                var section = Sections.ElementAt(subjectIndex);
                if (editContext == null)
                {
                    throw new InvalidOperationException();
                }

                editContext.NotifyReordered(args.TargetId);

                ListContext.EditContexts.Remove(editContext);
                Sections.Remove(section);

                if (targetIndex == -1)
                {
                    ListContext.EditContexts.Add(editContext);
                    Sections.Add(section);
                }
                else
                {
                    ListContext.EditContexts.Insert(targetIndex, editContext);
                    Sections.Insert(targetIndex, section);
                }

                var afterIds = ListContext.EditContexts.Select(x => x.Entity.Id).ToArray();
                var afterSections = Sections.Select(x => x.editContext.Entity.Id).ToArray();

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Mediator.NotifyEvent(this, new ExceptionEventArgs(ex));
            }
        }

    }
}

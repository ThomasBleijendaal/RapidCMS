using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Pages
{
    public abstract class BaseCollection : BasePage
    {
        [Inject] protected IPresentationService PresentationService { get; set; } = default!;
        [Inject] protected IInteractionService InteractionService { get; set; } = default!;
        [Inject] protected IUIResolverFactory UIResolverFactory { get; set; } = default!;

        protected ListContext? ListContext { get; set; }

        protected IEnumerable<ButtonUI>? Buttons { get; set; }
        protected IEnumerable<(EditContext editContext, IEnumerable<SectionUI> sections)>? Sections { get; set; }
        protected IEnumerable<TabUI>? Tabs { get; set; }

        protected IListUIResolver? UIResolver { get; set; }
        protected ListUI? ListUI { get; set; }

        protected int? ActiveTab { get; set; } = null;
        protected string? SearchTerm { get; set; } = null;
        protected int CurrentPage { get; set; } = 1;
        protected int? MaxPage { get; set; } = null;

        protected override async Task LoadDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            try
            {
                if (reloadEntityIds == null)
                {
                    var parentPath = GetParentPath();

                    UIResolver = await UIResolverFactory.GetListUIResolverAsync(GetUsageType(), CollectionAlias);
                    ListUI = UIResolver.GetListDetails();


                    CurrentPage = 1;
                    MaxPage = null;
                    ActiveTab = null;
                    SearchTerm = null;

                    var listContext = await LoadSectionsAsync();

                    Buttons = await UIResolver.GetButtonsForEditContextAsync(listContext.ProtoEditContext);
                    Tabs = await UIResolver.GetTabsAsync(listContext.ProtoEditContext);

                    ListContext = listContext;
                    Sections?.ForEach(x => x.editContext.OnFieldChanged += (s, a) => StateHasChanged());
                }
                else
                {
                    await ReloadSectionsAsync(reloadEntityIds);
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
            CurrentPage = page;

            await LoadSectionsAsync();
            StateHasChanged();
        }

        protected async Task SearchAsync(string? search)
        {
            CurrentPage = 1;

            SearchTerm = search;

            await LoadSectionsAsync();
            StateHasChanged();
        }

        protected async Task TabChangeAsync(int? tabId)
        {
            ActiveTab = tabId;

            CurrentPage = 1;

            await LoadSectionsAsync();
            StateHasChanged();
        }

        protected async Task<ListContext> LoadSectionsAsync()
        {
            var query = Query.Create(ListUI!.PageSize, CurrentPage, SearchTerm, ActiveTab);

            if (ListUI.OrderBys != null)
            {
                query.SetOrderByExpressions(ListUI.OrderBys);
            }

            var listContext = await PresentationService.GetEntitiesAsync(new GetEntitiesOfParentRequestModel
            {
                CollectionAlias = CollectionAlias,
                ParentPath = GetParentPath(),
                Query = query,
                UsageType = GetUsageType()
            });

            Sections = await listContext.EditContexts.ToListAsync(async editContext => (editContext, await UIResolver.GetSectionsForEditContextAsync(editContext)));

            if (!query.MoreDataAvailable)
            {
                MaxPage = CurrentPage;

                if (CurrentPage > 1 && !Sections.Any())
                {
                    CurrentPage--;
                    MaxPage = null;
                    await LoadSectionsAsync();
                }
            }
            if (MaxPage == CurrentPage && query.MoreDataAvailable)
            {
                MaxPage = null;
            }

            return listContext;
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
                        CollectionAlias = CollectionAlias,
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
                var command = await InteractionService.InteractAsync<PersistEntitiesRequestModel, ListViewCommandResponseModel>(new PersistEntitiesRequestModel
                {
                    ActionId = args.ViewModel.ButtonId,
                    CollectionAlias = CollectionAlias,
                    CustomData = args.Data,
                    ListContext = ListContext!,
                    UsageType = GetUsageType()
                });

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
                });

                await HandleViewCommandAsync(command);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}

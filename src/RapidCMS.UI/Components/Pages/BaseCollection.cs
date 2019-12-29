using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Resolvers.UI;
using RapidCMS.Common.Services;
using RapidCMS.Common.Services.UI;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Pages
{
    public abstract class BaseCollection : BasePage
    {
        [Inject] protected IEditContextService EditContextService { get; set; }
        [Inject] protected IEditorService EditorService { get; set; }

        protected EditContext? RootEditContext;

        protected IEnumerable<ButtonUI>? Buttons;
        protected IEnumerable<(EditContext editContext, IEnumerable<SectionUI> sections)>? Sections;
        protected IEnumerable<TabUI>? Tabs;

        protected IListUIResolver? UIResolver;
        protected ListUI? ListUI;

        protected int? ActiveTab = null;
        protected string? SearchTerm = null;
        protected int CurrentPage = 1;
        protected int? MaxPage = null;

        protected override async Task LoadDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            try
            {
                if (reloadEntityIds == null)
                {
                    var parentPath = GetParentPath();

                    var rootEditContext = await EditContextService.GetRootAsync(GetUsageType(), CollectionAlias, parentPath);

                    UIResolver = await EditorService.GetListUIResolverAsync(GetUsageType(), CollectionAlias);

                    ListUI = UIResolver.GetListDetails();
                    Buttons = await UIResolver.GetButtonsForEditContextAsync(rootEditContext);
                    Tabs = await UIResolver.GetTabsAsync(rootEditContext);

                    CurrentPage = 1;
                    MaxPage = null;
                    ActiveTab = null;
                    SearchTerm = null;

                    await LoadSectionsAsync();

                    RootEditContext = rootEditContext;
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
                RootEditContext = null;
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

        protected async Task LoadSectionsAsync()
        {
            var query = Query.Create(ListUI.PageSize, CurrentPage, SearchTerm, ActiveTab);

            if (ListUI.OrderBys != null)
            {
                query.SetOrderByExpressions(ListUI.OrderBys);
            }

            var editContexts = await EditContextService.GetEntitiesAsync(GetUsageType(), CollectionAlias, GetParentPath(), query);
            Sections = await editContexts.ToListAsync(async editContext => (editContext, await UIResolver.GetSectionsForEditContextAsync(editContext)));

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
                    var reloadedEditContext = await EditContextService.GetEntityAsync(x.editContext.UsageType, CollectionAlias, null, GetParentPath(), x.editContext.Entity.Id);
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
                var command = await EditContextService.ProcessListActionAsync(
                    GetUsageType(), 
                    CollectionAlias, 
                    GetParentPath(),
                    Sections.Select(x => x.editContext), 
                    args.ViewModel.ButtonId, 
                    args.Data);

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
                var command = await EditContextService.ProcessListActionAsync(
                    args.EditContext.UsageType, 
                    CollectionAlias, 
                    GetParentPath(), 
                    args.EditContext.Entity.Id, 
                    args.EditContext, 
                    args.ViewModel.ButtonId, 
                    args.Data);

                await HandleViewCommandAsync(command);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}

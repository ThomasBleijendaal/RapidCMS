using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Pages
{
    public abstract class BaseRelationCollection : BasePage
    {
        [Parameter] public IEntity RelatedEntity { get; set; }

        [Inject] protected IPersistenceService PersistenceService { get; set; }
        [Inject] protected IUIResolverFactory UIResolverFactory { get; set; }

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

        protected UsageType previousUsage;

        protected override async Task LoadDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            try
            {
                if (reloadEntityIds == null)
                {
                    // TODO: ListContext
                    var rootEditContext = default(EditContext); // await PersistenceService.GetRootAsync(GetUsageType(), CollectionAlias, default);

                    UIResolver = await UIResolverFactory.GetListUIResolverAsync(GetUsageType(), CollectionAlias);

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

        protected async void PageChangedAsync(int page)
        {
            CurrentPage = page;

            await LoadSectionsAsync();
            StateHasChanged();
        }

        protected async void SearchAsync(string? search)
        {
            CurrentPage = 1;

            SearchTerm = search;

            await LoadSectionsAsync();
            StateHasChanged();
        }

        protected async void TabChangeAsync(int? tabId)
        {
            ActiveTab = tabId;

            CurrentPage = 1;

            await LoadSectionsAsync();
            StateHasChanged();
        }

        protected async Task LoadSectionsAsync()
        {
            if (UIResolver == null)
            {
                return;
            }

            ListUI = UIResolver.GetListDetails();

            var query = Query.Create(ListUI.PageSize, CurrentPage, SearchTerm, ActiveTab);

            var editContexts = await PersistenceService.GetEntitiesAsync(GetUsageType(), CollectionAlias, default, query);
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
            if (ListUI == null || Sections == null)
            {
                return;
            }

            var newSections = await Sections.ToListAsync(async x =>
            {
                if (reloadEntityIds.Contains(x.editContext.Entity.Id))
                {
                    var reloadedEditContext = await PersistenceService.GetEntityAsync(x.editContext.UsageType, CollectionAlias, null, null, x.editContext.Entity.Id);
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
                var command = await PersistenceService.ProcessRelationActionAsync(
                    GetUsageType(),
                    CollectionAlias,
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
                var command = await PersistenceService.ProcessEntityActionAsync(
                    // args.EditContext.UsageType,
                    // CollectionAlias,
                    // RelatedEntity,
                    // args.EditContext.Entity.Id,
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

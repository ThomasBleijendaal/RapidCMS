using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Navigation
{
    public class NavigationState : IEquatable<NavigationState>, ICloneable
    {
        private readonly string? _collectionAlias;

        public NavigationState(string url, string queryString)
        {
            var urlItems = url.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (urlItems.Length == 0)
            {
                PageType = PageType.Dashboard;
            }
            else if (urlItems.Length == 3)
            {
                PageType = Enum.TryParse<PageType>(urlItems[0], true, out var pageType) ? pageType : PageType.Dashboard;
                UsageType = Enum.TryParse<UsageType>(urlItems[1], true, out var usageType) ? usageType : UsageType.View;
                _collectionAlias = urlItems[2] ?? null;
            }
            else if (urlItems.Length == 4)
            {
                PageType = Enum.TryParse<PageType>(urlItems[0], true, out var pageType) ? pageType : PageType.Dashboard;
                UsageType = Enum.TryParse<UsageType>(urlItems[1], true, out var usageType) ? usageType : UsageType.View;
                _collectionAlias = urlItems[2] ?? null;
                ParentPath = ParentPath.TryParse(urlItems[3]);
            }
            else if (urlItems.Length == 5)
            {
                PageType = Enum.TryParse<PageType>(urlItems[0], true, out var pageType) ? pageType : PageType.Dashboard;
                UsageType = Enum.TryParse<UsageType>(urlItems[1], true, out var usageType) ? usageType : UsageType.View;
                _collectionAlias = urlItems[2] ?? null;
                VariantAlias = urlItems[3] ?? null;
                Id = urlItems[4];
            }
            else if (urlItems.Length == 6)
            {
                PageType = Enum.TryParse<PageType>(urlItems[0], true, out var pageType) ? pageType : PageType.Dashboard;
                UsageType = Enum.TryParse<UsageType>(urlItems[1], true, out var usageType) ? usageType : UsageType.View;
                _collectionAlias = urlItems[2] ?? null;
                VariantAlias = urlItems[3] ?? null;
                ParentPath = ParentPath.TryParse(urlItems[4]);
                Id = urlItems[5];
            }

            var qs = HttpUtility.ParseQueryString(queryString);
            var tab = int.TryParse(qs.Get("tab"), out var t) ? t : default(int?);
            var searchTerm = qs.Get("q");
            var currentPage = int.TryParse(qs.Get("p"), out var p) ? p : 1;

            CollectionState = new CollectionState(tab, searchTerm, currentPage);
        }

        public NavigationState()
        {
            PageType = PageType.Dashboard;

            CollectionState = new CollectionState();
        }

        public NavigationState(string pageAlias, UsageType usageType)
        {
            PageType = PageType.Page;
            UsageType = usageType;
            _collectionAlias = pageAlias;

            CollectionState = new CollectionState();
        }

        public NavigationState(string collectionAlias, ParentPath? parentPath, UsageType usageType)
        {
            PageType = PageType.Collection;
            UsageType = usageType | ((parentPath == null) ? UsageType.Root : UsageType.NotRoot); // TODO: why root?
            _collectionAlias = collectionAlias;
            ParentPath = parentPath;

            CollectionState = new CollectionState();
        }

        public NavigationState(string collectionAlias, IRelated? related, UsageType usageType)
        {
            PageType = PageType.Collection;
            UsageType = usageType;
            _collectionAlias = collectionAlias;
            Related = related;

            CollectionState = new CollectionState();
        }

        public NavigationState(string collectionAlias, ParentPath? parentPath, IRelated? related, UsageType usageType)
        {
            PageType = PageType.Collection;
            UsageType = usageType | ((parentPath == null) ? UsageType.Root : UsageType.NotRoot);
            _collectionAlias = collectionAlias;
            ParentPath = parentPath;
            Related = related;

            CollectionState = new CollectionState();
        }

        public NavigationState(string collectionAlias, ParentPath? parentPath, string variantAlias, string? id, UsageType usageType)
        {
            PageType = PageType.Node;
            UsageType = usageType;
            _collectionAlias = collectionAlias;
            VariantAlias = variantAlias;
            ParentPath = parentPath;
            Id = id;

            CollectionState = new CollectionState();
        }

        private NavigationState(string? collectionAlias, PageType pageType, UsageType usageType, string? variantAlias, ParentPath? parentPath, IRelated? related, string? id)
        {
            _collectionAlias = collectionAlias;
            PageType = pageType;
            UsageType = usageType;
            VariantAlias = variantAlias;
            ParentPath = parentPath;
            Related = related;
            Id = id;

            CollectionState = new CollectionState();
        }

        public PageType PageType { get; }
        public UsageType UsageType { get; }
        public string CollectionAlias => _collectionAlias ?? throw new InvalidOperationException("CollectionAlias is null");
        public string? VariantAlias { get; }
        public ParentPath? ParentPath { get; }
        public IRelated? Related { get; set; }
        public string? Id { get; }

        public CollectionState CollectionState { get; set; }

        // TODO: make dictionary with key resolvable to nested state (so a history back event restores search / page / sorting in nested collections)
        public IList<NavigationState> NestedStates { get; } = new List<NavigationState>();

        public bool Equals(NavigationState? other)
        {
            return PageType == other?.PageType && 
                UsageType == other?.UsageType && 
                CollectionAlias == other?.CollectionAlias && 
                VariantAlias == other?.VariantAlias &&
                ParentPath?.ToPathString() == other?.ParentPath?.ToPathString() &&
                Related == other?.Related &&
                Id == other?.Id;
        }

        public override string ToString()
        {
            var nestedStates = NestedStates.Any() ? $"[{string.Join(";", NestedStates.Select(x => x.ToString()))}]" : null;

            return UriHelper.CombinePath(
                PageType.ToString().ToLower(),
                (UsageType.HasFlag(UsageType.Edit) ? UsageType.Edit : UsageType.View).ToString().ToLower(),
                CollectionAlias,
                VariantAlias,
                ParentPath?.ToPathString(),
                Id,
                nestedStates,
                CollectionState.ToString());
        }

        public object Clone() 
            => new NavigationState(CollectionAlias, PageType, UsageType, VariantAlias, ParentPath, Related, Id);
    }

    public record CollectionState(
        int? ActiveTab = default,
        string? SearchTerm = default,
        int CurrentPage = 1,
        int? MaxPage = default)
    {
        public override string ToString()
        {
            return UriHelper.CombineQueryString(
                ("tab", ActiveTab?.ToString()),
                ("q", SearchTerm),
                ("p", CurrentPage == 1 ? null : CurrentPage.ToString()));
        }
    }

    internal class NavigationStateProvider : INavigationStateProvider
    {
        private readonly IMediator _mediator;
        private readonly NavigationManager _navigationManager;
        private List<NavigationState> _states = new();

        public NavigationStateProvider(
            IMediator mediator,
            NavigationManager navigationManager)
        {
            _mediator = mediator;
            _navigationManager = navigationManager;
        }

        // TODO: this method should duplicate, otherwise the state will keep its page state
        public void AppendNavigationState(NavigationState state)
        {
            var newState = (NavigationState)state.Clone();

            _states.Add(newState);

            _mediator.NotifyEvent(this, new NavigationEventArgs(newState));

            UpdateUrl(newState);
        }

        public void NestNavigationState(NavigationState state, NavigationState nestedState)
        {
            var currentState = GetCurrentState();
            var stateToAddNestedStateTo = FindState(new[] { currentState }, state);

            stateToAddNestedStateTo?.NestedStates.Add(nestedState);

            UpdateUrl(currentState);
        }

        public NavigationState GetCurrentState()
        {
            if (_states.Count == 0)
            {
                _states.Add(new NavigationState());
            }

            return _states.Last();
        }

        public NavigationState Initialize(string url, string queryString)
        {
            var newState = new NavigationState(url, queryString);

            _states.Clear();

            _states.Add(newState);

            return newState;
        }

        public bool RemoveNavigationState()
        {
            var hasRemovedState = RemoveLastState();

            var newState = GetCurrentState();

            _mediator.NotifyEvent(this, new NavigationEventArgs(newState));

            _navigationManager.NavigateTo(newState.ToString());

            return hasRemovedState;
        }

        public void ReplaceNavigationState(NavigationState state)
        {
            RemoveLastState();
            AppendNavigationState(state);
        }

        private bool RemoveLastState()
        {
            var itemToRemove = _states.LastOrDefault();

            if (itemToRemove != null)
            {
                _states.Remove(itemToRemove);
                return true;
            }
            return false;
        }

        public void UpdateCollectionState(CollectionState state)
        {
            // TODO: add to correct navigation state
            var currentState = GetCurrentState();
            currentState.CollectionState = state;

            UpdateUrl(currentState);
        }

        public IView GetCurrentView(ListUI list)
        {
            var state = GetCurrentState();

            var view = View.Create(
                list.PageSize,
                state.CollectionState.CurrentPage,
                state.CollectionState.SearchTerm,
                state.CollectionState.ActiveTab,
                state.CollectionAlias);

            view.SetOrderBys(list.OrderBys);

            return view;
        }

        public bool TryProcessView(IView view, bool hasSections)
        {
            var currentState = GetCurrentState();

            var state = currentState.CollectionState;

            var overTheEnd = !view.MoreDataAvailable && state.CurrentPage > 1 && !hasSections;
            var atEnd = !view.MoreDataAvailable && !overTheEnd;
            var notAtEnd = view.MoreDataAvailable && state.MaxPage == state.CurrentPage;

            currentState.CollectionState = state with
            {
                MaxPage = notAtEnd || overTheEnd ? null : atEnd ? state.CurrentPage : state.MaxPage,
                CurrentPage = overTheEnd ? state.CurrentPage - 1 : state.CurrentPage
            };

            return !overTheEnd;
        }

        private void UpdateUrl(NavigationState state)
        {
            _navigationManager.NavigateTo(state.ToString());
        }

        private NavigationState? FindState(IEnumerable<NavigationState> states, NavigationState state)
        {
            return states.FirstOrDefault(x => x.Equals(state)) 
                ?? FindState(states.SelectManyNotNull(x => x.NestedStates), state);
        }
    }
}

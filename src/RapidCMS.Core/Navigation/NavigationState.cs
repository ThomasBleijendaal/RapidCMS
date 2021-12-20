using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Navigation
{
    public class NavigationState // TODO: : IEquatable<NavigationState>
    {
        public NavigationState(string url)
        {
            var urlItems = url.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (urlItems.Length == 0)
            {
                PageType = PageType.Dashboard;
            }
            else if (urlItems.Length == 3)
            {
                PageType = Enum.TryParse<PageType>(urlItems[0], out var pageType) ? pageType : PageType.Dashboard;
                UsageType = Enum.TryParse<UsageType>(urlItems[1], out var usageType) ? usageType : UsageType.View;
                CollectionAlias = urlItems[2] ?? null;
            }
            else if (urlItems.Length == 4)
            {
                PageType = Enum.TryParse<PageType>(urlItems[0], out var pageType) ? pageType : PageType.Dashboard;
                UsageType = Enum.TryParse<UsageType>(urlItems[1], out var usageType) ? usageType : UsageType.View;
                CollectionAlias = urlItems[2] ?? null;
                ParentPath = ParentPath.TryParse(urlItems[3]);
            }
            else if (urlItems.Length == 6)
            {
                PageType = Enum.TryParse<PageType>(urlItems[0], out var pageType) ? pageType : PageType.Dashboard;
                UsageType = Enum.TryParse<UsageType>(urlItems[1], out var usageType) ? usageType : UsageType.View;
                CollectionAlias = urlItems[2] ?? null;
                VariantAlias = urlItems[3] ?? null;
                ParentPath = ParentPath.TryParse(urlItems[4]);
                Id = urlItems[5];
            }
        }

        public NavigationState()
        {
            PageType = PageType.Dashboard;
        }

        public NavigationState(string pageAlias, UsageType usageType)
        {
            PageType = PageType.Page;
            UsageType = usageType;
            CollectionAlias = pageAlias;
        }

        public NavigationState(string collectionAlias, ParentPath? parentPath, UsageType usageType)
        {
            PageType = PageType.Collection;
            UsageType = usageType | ((parentPath == null) ? UsageType.Root : UsageType.NotRoot);
            CollectionAlias = collectionAlias;
            ParentPath = parentPath;
        }

        public NavigationState(string collectionAlias, ParentPath? parentPath, string variantAlias, string? id, UsageType usageType)
        {
            PageType = PageType.Node;
            UsageType = usageType;
            CollectionAlias = collectionAlias;
            VariantAlias = variantAlias;
            ParentPath = parentPath;
            Id = id;
        }

        public PageType PageType { get; }
        public UsageType UsageType { get; }
        public string? CollectionAlias { get; }
        public string? VariantAlias { get; }
        public ParentPath? ParentPath { get; }
        public string? Id { get; }

        public IView GetCurrentView(ListUI list)
            // TODO: implement + implement order bys
            => View.Create(list.PageSize, 1, default, default, CollectionAlias);

        // TODO: make dictionary with key resolvable to nested state (so a history back event restores search / page / sorting in nested collections)
        public IEnumerable<NavigationState> NestedStates { get; } = new List<NavigationState>();

        //public bool Equals(NavigationState? other)
        //{
        //    // TODO; compare by tostring?
        //    throw new NotImplementedException();
        //}



        public override string ToString()
            => UriHelper.Combine(
                PageType.ToString().ToLower(),
                (UsageType.HasFlag(UsageType.Edit) ? UsageType.Edit : UsageType.View).ToString().ToLower(),
                CollectionAlias,
                VariantAlias,
                ParentPath?.ToPathString(),
                Id);
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


        public void AppendNavigationState(NavigationState state)
        {
            _states.Add(state);

            _mediator.NotifyEvent(this, new NavigationEventArgs(state));

            _navigationManager.NavigateTo(state.ToString());
        }

        public NavigationState GetCurrentState()
        {
            if (_states.Count == 0)
            {
                _states.Add(new NavigationState());
            }

            return _states.Last();
        }

        public NavigationState Initialize(string url)
        {
            var newState = new NavigationState(url);

            _states.Clear();

            _states.Add(newState);

            return newState;
        }

        public void RemoveNavigationState()
        {
            RemoveLastState();

            var newState = GetCurrentState();

            _mediator.NotifyEvent(this, new NavigationEventArgs(newState));

            _navigationManager.NavigateTo(newState.ToString());
        }

        public void ReplaceNavigationState(NavigationState state)
        {
            RemoveLastState();
            AppendNavigationState(state);
        }

        private void RemoveLastState()
        {
            var itemToRemove = _states.LastOrDefault();

            if (itemToRemove != null)
            {
                _states.Remove(itemToRemove);
            }
        }
    }
}

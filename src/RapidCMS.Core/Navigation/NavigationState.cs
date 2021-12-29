using System;
using System.Collections.Generic;
using System.Web;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;

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
                // TODO: test if url with 5 is really this kind of url
                ParentPath = ParentPath.TryParse(urlItems[4]);
                Id = ParentPath == null ? urlItems[4] : null;
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
            UsageType = usageType; // | ((parentPath == null) ? UsageType.Root : UsageType.NotRoot); // TODO: why root?
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

        public NavigationState(string collectionAlias, ParentPath? parentPath, IRelated? related, UsageType usageType, PageType pageType)
        {
            PageType = pageType;
            UsageType = usageType; // | ((parentPath == null) ? UsageType.Root : UsageType.NotRoot);
            _collectionAlias = collectionAlias;
            ParentPath = parentPath;
            Related = related;

            CollectionState = new CollectionState();
        }

        public NavigationState(string collectionAlias, ParentPath? parentPath, string variantAlias, IRelated? related, UsageType usageType)
        {
            PageType = PageType.Node;
            UsageType = usageType;
            _collectionAlias = collectionAlias;
            VariantAlias = variantAlias;
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

        public NavigationState(string collectionAlias, ParentPath? parentPath, string variantAlias, IRelated? related, string? id, UsageType usageType)
        {
            PageType = PageType.Node;
            UsageType = usageType;
            _collectionAlias = collectionAlias;
            VariantAlias = variantAlias;
            ParentPath = parentPath;
            Related = related;
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

        public override string ToString() => ToString(false);

        private string ToString(bool nested)
        {
            // var nestedStates = NestedStates.Any() ? $"[{string.Join(";", NestedStates.Select(x => x.ToString(true)))}]" : null;

            return UriHelper.CombinePath(
                PageType.ToString().ToLower(),
                (UsageType.HasFlag(UsageType.Edit) ? UsageType.Edit : 
                    UsageType.HasFlag(UsageType.New) ? UsageType.New :
                    UsageType.View).ToString().ToLower(),
                CollectionAlias,
                VariantAlias,
                ParentPath?.ToPathString(),
                Id,
                nested ? default : CollectionState.ToString());
        }

        public object Clone()
            => new NavigationState(CollectionAlias, PageType, UsageType, VariantAlias, ParentPath, Related, Id);
    }
}

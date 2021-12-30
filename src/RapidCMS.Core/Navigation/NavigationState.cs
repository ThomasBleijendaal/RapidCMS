using System;
using System.Collections.Generic;
using System.Linq;
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
            else
            {
                PageType = Enum.TryParse<PageType>(urlItems[0], true, out var pageType) ? pageType : PageType.Dashboard;

                if (urlItems.Length > 1)
                {
                    switch (PageType)
                    {
                        case PageType.Collection:

                            UsageType = Enum.TryParse<UsageType>(urlItems.ElementAtOrDefault(1), true, out var usageType1) ? usageType1 : UsageType.View;
                            _collectionAlias = urlItems.ElementAtOrDefault(2);
                            ParentPath = ParentPath.TryParse(urlItems.ElementAtOrDefault(3));
                            break;

                        case PageType.Node:

                            UsageType = Enum.TryParse<UsageType>(urlItems.ElementAtOrDefault(1), true, out var usageType2) ? usageType2 : UsageType.View;
                            _collectionAlias = urlItems.ElementAtOrDefault(2);
                            VariantAlias = urlItems.ElementAtOrDefault(3);
                            ParentPath = ParentPath.TryParse(urlItems.ElementAtOrDefault(4));
                            Id = urlItems.ElementAtOrDefault(5);
                            break;

                        case PageType.Page:

                            _collectionAlias = urlItems.ElementAtOrDefault(1);

                            break;
                    }
                }
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

        public NavigationState(PageType pageType)
        {
            PageType = pageType;

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

        public bool HasCollectionAlias => !string.IsNullOrEmpty(_collectionAlias);

        public CollectionState CollectionState { get; set; }

        // TODO: make dictionary with key resolvable to nested state (so a history back event restores search / page / sorting in nested collections)
        public IList<NavigationState> NestedStates { get; } = new List<NavigationState>();

        public bool Equals(NavigationState? other)
        {
            return PageType == other?.PageType &&
                UsageType == other?.UsageType &&
                _collectionAlias == other?._collectionAlias &&
                VariantAlias == other?.VariantAlias &&
                ParentPath?.ToPathString() == other?.ParentPath?.ToPathString() &&
                Related == other?.Related &&
                Id == other?.Id;
        }

        public bool IsSimilar(NavigationState? other)
        {
            return PageType == other?.PageType &&
                // UsageType == other?.UsageType &&
                _collectionAlias == other?._collectionAlias &&
                VariantAlias == other?.VariantAlias &&
                ParentPath?.ToPathString() == other?.ParentPath?.ToPathString() &&
                Related == other?.Related &&
                Id == other?.Id;
        }

        public override string ToString() => ToString(false);

        private string ToString(bool nested)
        {
            // var nestedStates = NestedStates.Any() ? $"[{string.Join(";", NestedStates.Select(x => x.ToString(true)))}]" : null;

            var usageType = (UsageType.HasFlag(UsageType.Edit) ? UsageType.Edit :
                    UsageType.HasFlag(UsageType.New) ? UsageType.New :
                    UsageType.View).ToString().ToLower();

            return PageType switch
            {
                PageType.Collection => UriHelper.CombinePath("collection", usageType, _collectionAlias, ParentPath?.ToPathString(), nested ? default : CollectionState.ToString()),
                PageType.Dashboard => "",
                PageType.Error => "error",
                PageType.Node => UriHelper.CombinePath("node", usageType, _collectionAlias, VariantAlias, ParentPath?.ToPathString() ?? "-", Id),
                PageType.Page => $"page/{_collectionAlias}",
                PageType.Unauthorized => "unauthorized",
                _ => ""
            };
        }

        public object Clone()
            => new NavigationState(_collectionAlias, PageType, UsageType, VariantAlias, ParentPath, Related, Id)
            { 
                CollectionState = CollectionState 
            };
    }
}

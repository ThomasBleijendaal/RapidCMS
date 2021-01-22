using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request
{
    public class NavigationRequest
    {
        public static NavigationRequest NavigateToEntity(string collectionAlias, string id, IParent? parent)
        {
            return new NavigationRequest(collectionAlias)
            {
                Id = id,
                ParentPath = parent?.GetParentPath(),
                IsEdit = true
            };
        }

        public static NavigationRequest NavigateToCreateNewEntity(string collectionAlias, IParent? parent, Type? entityVariant = null)
        {
            return new NavigationRequest(collectionAlias)
            {
                ParentPath = parent?.GetParentPath(),
                IsEdit = true,
                IsNew = true,
                VariantAlias = entityVariant == null ? null : AliasHelper.GetEntityVariantAlias(entityVariant)
            };
        }

        public static NavigationRequest NavigateToCollection(string collectionAlias, IParent? parent)
        {
            return new NavigationRequest(collectionAlias)
            {
                ParentPath = parent?.GetParentPath(),
                IsEdit = true,
                IsList = true
            };
        }

        public static NavigationRequest NavigateToDetails(string detailAlias, string id, IParent? parent)
        {
            return new NavigationRequest(detailAlias)
            {
                ParentPath = parent?.GetParentPath(),
                Id = id,
                IsEdit = true
            };
        }

        public static NavigationRequest NavigateToPage(string pageName)
        {
            return new NavigationRequest(pageName.ToUrlFriendlyString())
            {
                IsPage = true
            };
        }

        private NavigationRequest(string collectionAlias)
        {
            CollectionAlias = collectionAlias;
        }

        internal string CollectionAlias { get; private set; }

        internal string? VariantAlias { get; private set; }

        internal string? Id { get; set; }

        internal ParentPath? ParentPath { get; set; }

        internal bool IsEdit { get; set; }

        internal bool IsNew { get; set; }

        internal bool IsList { get; set; }

        internal bool IsPage { get; set; }
    }
}

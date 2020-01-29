using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Helpers
{
    internal static class UriHelper
    {
        public static string Node(string action, string collectionAlias, IEntityVariantSetup entityVariant, ParentPath? parentPath, string? id)
        {
            return Node(action, collectionAlias, entityVariant.Alias, parentPath, id);
        }

        public static string Node(string action, string collectionAlias, string entityVariantAlias, ParentPath? parentPath, string? id)
        {
            var path = parentPath?.ToPathString();

            return $"/node/{action}{path.ToUriPart()}/{collectionAlias}/{entityVariantAlias}{id.ToUriPart()}";
        }

        public static string Collection(string action, string collectionAlias, ParentPath? parentPath)
        {
            var path = parentPath?.ToPathString();

            return $"/collection/{action}{path.ToUriPart()}/{collectionAlias}";
        }

        private static string ToUriPart(this string? nullableString)
        {
            return string.IsNullOrWhiteSpace(nullableString) ? "" : $"/{nullableString}";
        }
    }
}

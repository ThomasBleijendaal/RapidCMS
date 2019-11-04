using RapidCMS.Common.Data;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Helpers
{
    internal static class UriHelper
    {
        public static string Node(string action, string collectionAlias, EntityVariant entityVariant, ParentPath? parentPath, string? id)
        {
            var path = parentPath?.ToPath();

            return $"/{action}{path.ToUriPart()}/{collectionAlias}/entity/{entityVariant.Alias}{id.ToUriPart()}";
        }

        public static string Collection(string action, string collectionAlias, ParentPath? parentPath)
        {
            var path = parentPath?.ToPath();

            return $"/{action}{path.ToUriPart()}/{collectionAlias}";
        }

        private static string ToUriPart(this string? nullableInt)
        {
            return nullableInt != null ? $"/{nullableInt}" : "";
        }
    }
}

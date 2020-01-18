using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Helpers
{
    internal static class UriHelper
    {
        public static string Node(string action, string collectionAlias, EntityVariantSetup entityVariant, ParentPath? parentPath, string? id)
        {
            var path = parentPath?.ToPathString();

            return $"/{action}{path.ToUriPart()}/{collectionAlias}/entity/{entityVariant.Alias}{id.ToUriPart()}";
        }

        public static string Collection(string action, string collectionAlias, ParentPath? parentPath)
        {
            var path = parentPath?.ToPathString();

            return $"/{action}{path.ToUriPart()}/{collectionAlias}";
        }

        private static string ToUriPart(this string? nullableString)
        {
            return string.IsNullOrWhiteSpace(nullableString) ? "" : $"/{nullableString}";
        }
    }
}

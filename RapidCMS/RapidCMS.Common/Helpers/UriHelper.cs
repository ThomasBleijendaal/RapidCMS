using RapidCMS.Common.Models;


namespace RapidCMS.Common.Helpers
{
    internal static class UriHelper
    {
        // TODO: find better solution than /entity/
        public static string Node(string action, string collectionAlias, EntityVariant entityVariant, string? parentId, string? id)
        {
            return $"/node/{action}{parentId.ToUriPart()}/{collectionAlias}/entity/{entityVariant.Alias}{id.ToUriPart()}";
        }

        public static string Collection(string action, string collectionAlias, string? parentId)
        {
            return $"/collection/{action}/{collectionAlias}{parentId.ToUriPart()}";
        }

        private static string ToUriPart(this string? nullableInt)
        {
            return nullableInt != null ? $"/{nullableInt}" : "";
        }
    }
}

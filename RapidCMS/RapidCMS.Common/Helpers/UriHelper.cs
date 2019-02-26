using System;
using System.Collections.Generic;
using System.Text;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Helpers
{
    public static class UriHelper
    {
        public static string Node(string action, string collectionAlias, EntityVariant entityVariant, int? parentId, int? id)
        {
            return $"/node/{action}{parentId.ToUriPart()}/{collectionAlias}/{entityVariant.Alias}{id.ToUriPart()}";
        }

        public static string Collection(string action, string collectionAlias, int? parentId)
        {
            return $"/collection/{action}/{collectionAlias}{parentId.ToUriPart()}";
        }

        private static string ToUriPart(this int? nullableInt)
        {
            return nullableInt.HasValue ? $"/{nullableInt.Value}" : "";
        }
    }
}

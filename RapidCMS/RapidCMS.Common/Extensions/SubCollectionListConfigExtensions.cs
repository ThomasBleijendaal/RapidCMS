using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class SubCollectionListConfigExtensions
    {
        public static SubCollectionList ToSubCollectionList(this SubCollectionListConfig subCollection)
        {
            return new SubCollectionList
            {
                Index = subCollection.Index,
                CollectionAlias = subCollection.CollectionAlias
            };
        }
    }
}

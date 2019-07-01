using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class RelatedCollectionListConfigExtensions
    {
        public static RelatedCollectionList ToRelatedCollectionList(this RelatedCollectionListConfig relatedCollection)
        {
            return new RelatedCollectionList
            {
                Index = relatedCollection.Index,
                CollectionAlias = relatedCollection.CollectionAlias
            };
        }
    }
}

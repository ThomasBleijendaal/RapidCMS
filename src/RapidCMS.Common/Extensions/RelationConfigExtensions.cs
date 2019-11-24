using System;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class RelationConfigExtensions
    {
        public static Relation ToRelation(this RelationConfig config)
        {
            return config switch
            {
                CollectionRelationConfig collectionConfig => (Relation)new CollectionRelation(
                    collectionConfig.CollectionAlias!, 
                    collectionConfig.RelatedEntityType!, 
                    collectionConfig.IdProperty!, 
                    collectionConfig.DisplayProperties!)
                {
                    RepositoryParentSelector = collectionConfig.RepositoryParentProperty,
                    RelatedElementsGetter = collectionConfig.RelatedElementsGetter
                },
                DataProviderRelationConfig dataProviderConfig => new DataProviderRelation(
                    dataProviderConfig.DataCollectionType),
                _ => throw new InvalidOperationException("Invalid RelationConfig")
            };
        }
    }
}

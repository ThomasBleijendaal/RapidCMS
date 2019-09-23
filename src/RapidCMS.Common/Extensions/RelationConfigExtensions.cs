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
                CollectionRelationConfig collectionConfig => (Relation)new CollectionRelation
                {
                    CollectionAlias = collectionConfig.CollectionAlias,
                    RelatedEntityType = collectionConfig.RelatedEntityType,
                    DisplayProperties = collectionConfig.DisplayProperties,
                    IdProperty = collectionConfig.IdProperty,
                    RepositoryParentIdProperty = collectionConfig.RepositoryParentIdProperty,
                    RelatedElementsGetter = collectionConfig.RelatedElementsGetter
                },
                DataProviderRelationConfig dataProviderConfig => new DataProviderRelation
                {
                    DataCollectionType = dataProviderConfig.DataCollectionType
                },
                _ => throw new InvalidOperationException("Invalid RelationConfig")
            };
        }
    }
}

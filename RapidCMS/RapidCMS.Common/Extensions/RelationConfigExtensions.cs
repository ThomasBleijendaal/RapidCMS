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
                CollectionRelationConfig collectionConfig => new CollectionRelation
                {
                    CollectionAlias = collectionConfig.CollectionAlias,
                    RelatedEntityType = collectionConfig.RelatedEntityType,
                    DisplayProperty = collectionConfig.DisplayProperty,
                    IdProperty = collectionConfig.IdProperty,
                    RepositoryParentIdProperty = collectionConfig.RepositoryParentIdProperty
                },
                DataProviderRelationConfig dataProviderConfig => new DataProviderRelation
                {
                    DataCollectionType = dataProviderConfig.DataCollectionType
                },
                _ => default(Relation)
            };
        }
    }
}

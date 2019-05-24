using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class RelationConfigExtensions
    {
        public static OneToManyRelation ToOneToManyRelation(this OneToManyRelationConfig config)
        {
            return config switch
            {
                OneToManyRelationCollectionConfig collectionConfig => new OneToManyCollectionRelation
                {
                    CollectionAlias = collectionConfig.CollectionAlias,
                    DisplayProperty = collectionConfig.DisplayProperty,
                    IdProperty = collectionConfig.IdProperty
                },
                OneToManyRelationDataProviderConfig dataProviderConfig => new OneToManyDataProviderRelation
                {
                    DataCollectionType = dataProviderConfig.DataCollectionType
                },
                _ => default(OneToManyRelation)
            };
        }
    }
}

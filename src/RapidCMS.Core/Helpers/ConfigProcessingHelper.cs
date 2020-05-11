using System;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Helpers
{
    [Obsolete]
    internal static class ConfigProcessingHelper
    {
        internal static RelationSetup ProcessRelation(RelationConfig config)
        {
            return config switch
            {
                CollectionRelationConfig collectionConfig => (RelationSetup)new CollectionRelationSetup(
                    collectionConfig.CollectionAlias,
                    collectionConfig.RelatedRepositoryType,
                    collectionConfig.RelatedEntityType!,
                    collectionConfig.IdProperty!,
                    collectionConfig.DisplayProperties!)
                {
                    RepositoryParentSelector = collectionConfig.RepositoryParentProperty,
                    RelatedElementsGetter = collectionConfig.RelatedElementsGetter
                },
                DataProviderRelationConfig dataProviderConfig => new DataProviderRelationSetup(
                    dataProviderConfig.DataCollectionType),
                _ => throw new InvalidOperationException("Invalid RelationConfig")
            };
        }
    }
}

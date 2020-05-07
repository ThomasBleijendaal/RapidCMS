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

        public static FieldSetup ProcessField(FieldConfig field)
        {
            return field switch
            {
                _ when field.EditorType == EditorType.Custom && field.Property != null => new CustomPropertyFieldSetup(field, field.CustomType!),
                _ when field.EditorType != EditorType.None && field.Property != null => new PropertyFieldSetup(field),
                _ when field.DisplayType != DisplayType.None && field.Property != null => new ExpressionFieldSetup(field, field.Property),
                _ when field.DisplayType == DisplayType.Custom && field.Expression != null => new CustomExpressionFieldSetup(field, field.Expression, field.CustomType!),
                _ when field.DisplayType != DisplayType.None && field.Expression != null => new ExpressionFieldSetup(field, field.Expression),
                _ => throw new InvalidOperationException()
            };
        }
    }
}

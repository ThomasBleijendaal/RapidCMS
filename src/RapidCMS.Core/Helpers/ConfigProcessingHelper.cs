using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Helpers
{
    internal static class ConfigProcessingHelper
    {
        internal static List<CollectionSetup> ProcessCollections(this ICollectionConfig root)
        {
            var list = new List<CollectionSetup>();

            foreach (var configReceiver in root.Collections.Cast<CollectionConfig>())
            {
                var collection = new CollectionSetup(
                    configReceiver.Icon,
                    configReceiver.Name,
                    configReceiver.Alias,
                    new EntityVariantSetup(configReceiver.EntityVariant),
                    configReceiver.RepositoryType,
                    configReceiver.Recursive)
                {
                    DataViews = configReceiver.DataViews,
                    DataViewBuilder = configReceiver.DataViewBuilder
                };

                if (configReceiver.SubEntityVariants.Any())
                {
                    collection.SubEntityVariants = configReceiver.SubEntityVariants.ToList(variant => new EntityVariantSetup(variant));
                }

                collection.TreeView = configReceiver.TreeView == null ? null : new TreeViewSetup(configReceiver.TreeView);

                collection.ListView = configReceiver.ListView == null ? null : new ListSetup(configReceiver.ListView, collection);
                collection.ListEditor = configReceiver.ListEditor == null ? null : new ListSetup(configReceiver.ListEditor, collection);

                collection.NodeView = configReceiver.NodeView == null ? null : new NodeSetup(configReceiver.NodeView, collection);
                collection.NodeEditor = configReceiver.NodeEditor == null ? null : new NodeSetup(configReceiver.NodeEditor, collection);

                collection.Collections = configReceiver.ProcessCollections();

                list.Add(collection);
            }

            return list;
        }

        internal static RelationSetup ProcessRelation(RelationConfig config)
        {
            return config switch
            {
                CollectionRelationConfig collectionConfig => (RelationSetup)new CollectionRelationSetup(
                    collectionConfig.CollectionAlias!,
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

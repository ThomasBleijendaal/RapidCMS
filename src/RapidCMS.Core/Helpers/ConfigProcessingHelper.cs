using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Helpers
{
    internal static class ConfigProcessingHelper
    {
        //internal static List<ITreeElementSetup> ProcessCollections(this ICollectionConfig root)
        //{
        //    var list = new List<ITreeElementSetup>();

        //    foreach (var element in root.CollectionsAndPages)
        //    {
        //        if (element is CollectionConfig collectionConfigReceiver) {

        //            var collection = new CollectionSetup(
        //                collectionConfigReceiver.Icon,
        //                collectionConfigReceiver.Name,
        //                collectionConfigReceiver.Alias,
        //                new EntityVariantSetup(collectionConfigReceiver.EntityVariant),
        //                collectionConfigReceiver.RepositoryType,
        //                collectionConfigReceiver.Recursive)
        //            {
        //                DataViews = collectionConfigReceiver.DataViews,
        //                DataViewBuilder = collectionConfigReceiver.DataViewBuilder
        //            };

        //            if (collectionConfigReceiver.SubEntityVariants.Any())
        //            {
        //                collection.SubEntityVariants = collectionConfigReceiver.SubEntityVariants.ToList(variant => new EntityVariantSetup(variant));
        //            }

        //            collection.TreeView = collectionConfigReceiver.TreeView == null ? null : new TreeViewSetup(collectionConfigReceiver.TreeView);

        //            collection.ListView = collectionConfigReceiver.ListView == null ? null : new ListSetup(collectionConfigReceiver.ListView, collection);
        //            collection.ListEditor = collectionConfigReceiver.ListEditor == null ? null : new ListSetup(collectionConfigReceiver.ListEditor, collection);

        //            collection.NodeView = collectionConfigReceiver.NodeView == null ? null : new NodeSetup(collectionConfigReceiver.NodeView, collection);
        //            collection.NodeEditor = collectionConfigReceiver.NodeEditor == null ? null : new NodeSetup(collectionConfigReceiver.NodeEditor, collection);

        //            // nested pages are not supported
        //            collection.Collections = collectionConfigReceiver.ProcessCollections().SelectNotNull(x => x as CollectionSetup).ToList();

        //            list.Add(collection);
        //        }
        //        else if (element is IPageConfig pageConfigReceiver)
        //        {
        //            list.Add(new PageRegistrationSetup(pageConfigReceiver));
        //        }
        //    }

        //    return list;
        //}

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

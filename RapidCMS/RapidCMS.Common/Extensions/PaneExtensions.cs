using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Extensions
{
    internal static class PaneExtensions
    {
        internal static IEnumerable<(IPropertyMetadata property, IDataCollection relation, IRelationValidator? validator)> GetDataCollections(this Node node, IServiceProvider serviceProvider)
        {
            return GetDataCollections(node.EditorPanes, serviceProvider);
        }

        internal static IEnumerable<(IPropertyMetadata property, IDataCollection relation, IRelationValidator? validator)> GetDataCollections(this List listEditor, IServiceProvider serviceProvider)
        {
            return GetDataCollections(listEditor.Panes, serviceProvider);
        }

        internal static IEnumerable<(IPropertyMetadata property, IDataCollection relation, IRelationValidator? validator)> GetDataCollections(this List<Pane>? panes, IServiceProvider serviceProvider)
        {
            if (panes == null)
            {
                return Enumerable.Empty<(IPropertyMetadata property, IDataCollection relation, IRelationValidator? validator)>();
            }

            return panes
                .SelectMany(pane => pane.Fields)
                .WhereAs(field => field as PropertyField)
                .Where(field => field.Relation != null)
                .Select(field =>
                {
                    switch (field.Relation)
                    {
                        case CollectionRelation collectionRelation:

                            // TODO: horrible serviceProvider usage
                            var cr = serviceProvider.GetService<Root>(typeof(Root));
                            var repo = cr.GetRepository(collectionRelation.CollectionAlias);
                            if (repo == null)
                            {
                                throw new InvalidOperationException($"Field {field.Property.PropertyName} has incorrectly configure relation, cannot find repository for collection alias {collectionRelation.CollectionAlias}.");
                            }

                            IDataCollection provider = new CollectionDataProvider(
                                repo,
                                collectionRelation.RelatedEntityType,
                                collectionRelation.RepositoryParentIdProperty,
                                collectionRelation.IdProperty,
                                collectionRelation.DisplayProperties);

                            IRelationValidator? validator = collectionRelation.ValidationFunction != null
                                ? new CollectionDataValidator(field.Property, collectionRelation.ValidationFunction)
                                : null;

                            return (field.Property, provider, validator);

                        case DataProviderRelation dataProviderRelation:

                            return (field.Property, serviceProvider.GetService<IDataCollection>(dataProviderRelation.DataCollectionType), default);

                        default:

                            throw new InvalidOperationException();
                    }
                }); 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.Config
{
    public class RelatedCollectionListConfig
    {
        public RelatedCollectionListConfig(string collectionAlias)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
        }

        internal int Index { get; set; }

        internal string CollectionAlias { get; set; }

        internal IPropertyMetadata RelatedElements { get; set; }
    }

    public class RelatedCollectionListConfig<TEntity, TRelatedEntity> : RelatedCollectionListConfig
        where TRelatedEntity : IEntity
        where TEntity : IEntity
    {
        public RelatedCollectionListConfig(string collectionAlias) : base(collectionAlias)
        {
        }

        public RelatedCollectionListConfig<TEntity, TRelatedEntity> SetRelatedItems(Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> relatedElements)
        {
            RelatedElements = PropertyMetadataHelper.GetPropertyMetadata(relatedElements) ?? throw new InvalidPropertyExpressionException(nameof(relatedElements));

            return this;
        }
        
        // TODO: config options for:
        // - entity selection (what should be displayed in select)
        // - which entities are bounded

    }
}

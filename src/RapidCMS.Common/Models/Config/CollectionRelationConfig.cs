using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.Config
{
    internal class CollectionRelationConfig : RelationConfig
    {
        internal string? CollectionAlias { get; set; }
        internal Type? RelatedEntityType { get; set; }
        internal IPropertyMetadata? RelatedElementsGetter { get; set; }
        internal IPropertyMetadata? RepositoryParentIdProperty { get; set; }
        internal IPropertyMetadata? IdProperty { get; set; }
        internal List<IExpressionMetadata>? DisplayProperties { get; set; }
    }

    internal class CollectionRelationConfig<TEntity, TRelatedEntity> : CollectionRelationConfig, ICollectionRelationConfig<TEntity, TRelatedEntity>
    {
        public CollectionRelationConfig()
        {
            RelatedEntityType = typeof(TRelatedEntity);
        }

        public CollectionRelationConfig(IPropertyMetadata relatedElements)
        {
            RelatedEntityType = typeof(TRelatedEntity);
            RelatedElementsGetter = relatedElements;
        }

        public ICollectionRelationConfig<TEntity, TRelatedEntity> SetElementIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
        {
            IdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

            return this;
        }

        public ICollectionRelationConfig<TEntity, TRelatedEntity> SetElementDisplayProperties(params Expression<Func<TRelatedEntity, string>>[] propertyExpressions)
        {
            DisplayProperties = propertyExpressions
                .Select(propertyExpression => PropertyMetadataHelper.GetExpressionMetadata(propertyExpression) ?? throw new InvalidExpressionException(nameof(propertyExpression)))
                .ToList();

            return this;
        }

        public ICollectionRelationConfig<TEntity, TRelatedEntity> SetRepositoryParentIdProperty(Expression<Func<TEntity, string>> propertyExpression)
        {
            RepositoryParentIdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

            return this;
        }
    }
}

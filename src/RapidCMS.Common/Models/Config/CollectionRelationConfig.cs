using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.Config
{
    public class CollectionRelationConfig : RelationConfig
    {
        internal string CollectionAlias { get; set; }
        internal Type RelatedEntityType { get; set; }
        internal IPropertyMetadata? RelatedElementsGetter { get; set; }
        internal IPropertyMetadata? RepositoryParentIdProperty { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal List<IExpressionMetadata> DisplayProperties { get; set; }
    }

    public class CollectionRelationConfig<TEntity, TRelatedEntity> : CollectionRelationConfig
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

        public CollectionRelationConfig<TEntity, TRelatedEntity> SetElementIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
        {
            IdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

            return this;
        }

        public CollectionRelationConfig<TEntity, TRelatedEntity> SetElementDisplayProperties(params Expression<Func<TRelatedEntity, string>>[] propertyExpressions)
        {
            DisplayProperties = propertyExpressions
                .Select(propertyExpression => PropertyMetadataHelper.GetExpressionMetadata(propertyExpression) ?? throw new InvalidExpressionException(nameof(propertyExpression)))
                .ToList();

            return this;
        }

        // HACK: hardcoded IEntity.Id type (string)
        public CollectionRelationConfig<TEntity, TRelatedEntity> SetRepositoryParentIdProperty(Expression<Func<TEntity, string>> propertyExpression)
        {
            RepositoryParentIdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

            return this;
        }
    }
}

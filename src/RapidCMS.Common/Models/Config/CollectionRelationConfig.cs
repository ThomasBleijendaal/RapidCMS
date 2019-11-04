using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
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
        internal IPropertyMetadata? RepositoryParentProperty { get; set; }
        internal IPropertyMetadata? IdProperty { get; set; }
        internal List<IExpressionMetadata>? DisplayProperties { get; set; }
    }

    internal class CollectionRelationConfig<TEntity, TRelatedEntity> : CollectionRelationConfig, ICollectionRelationConfig<TEntity, TRelatedEntity>
        where TEntity : IEntity
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

        public ICollectionRelationConfig<TEntity, TRelatedEntity> SetRepositoryParent(Expression<Func<IParent, IParent?>> propertyExpression)
        {
            RepositoryParentProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

            return this;
        }

        public ICollectionRelationConfig<TEntity, TRelatedEntity> SetEntityAsParent()
        {
            Expression<Func<(IParent? parent, IEntity entity), IParent>> expression = ((IParent? parent, IEntity entity) combined) => new ParentEntity(combined.parent, combined.entity);

            RepositoryParentProperty = PropertyMetadataHelper.GetPropertyMetadata(expression);

            return this;
        }
    }
}

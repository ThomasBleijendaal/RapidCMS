using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Config
{
    internal class CollectionRelationConfig : RelationConfig
    {
        protected internal string? CollectionAlias { get; protected set; }
        internal Type? RelatedEntityType { get; set; }
        protected internal Type? RelatedRepositoryType { get; protected set; }
        internal IPropertyMetadata? RelatedElementsGetter { get; set; }
        internal IPropertyMetadata? RepositoryParentProperty { get; set; }
        internal IPropertyMetadata? IdProperty { get; set; }
        internal List<IExpressionMetadata>? DisplayProperties { get; set; }
    }

    internal class CollectionRelationConfig<TEntity, TRelatedEntity> : CollectionRelationConfig, ICollectionRelationConfig<TEntity, TRelatedEntity>
        where TEntity : IEntity
    {
        public CollectionRelationConfig(string collectionAlias)
        {
            CollectionAlias = collectionAlias;
            RelatedEntityType = typeof(TRelatedEntity);
        }

        public CollectionRelationConfig(Type relatedRepositoryType)
        {
            CollectionAlias = Guid.NewGuid().ToString();
            RelatedEntityType = typeof(TRelatedEntity);
            RelatedRepositoryType = relatedRepositoryType;
        }

        public CollectionRelationConfig(string collectionAlias, IPropertyMetadata relatedElements)
        {
            CollectionAlias = collectionAlias;
            RelatedEntityType = typeof(TRelatedEntity);
            RelatedElementsGetter = relatedElements;
        }

        public CollectionRelationConfig(Type relatedRepositoryType, IPropertyMetadata relatedElements)
        {
            CollectionAlias = Guid.NewGuid().ToString();
            RelatedEntityType = typeof(TRelatedEntity);
            RelatedRepositoryType = relatedRepositoryType;
            RelatedElementsGetter = relatedElements;
        }

        public ICollectionRelationConfig<TEntity, TRelatedEntity> SetElementIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
        {
            IdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

            return this;
        }

        public ICollectionRelationConfig<TEntity, TRelatedEntity> SetElementDisplayProperties(params Expression<Func<TRelatedEntity, string?>>[] propertyExpressions)
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
            if (string.IsNullOrEmpty(CollectionAlias))
            {
                throw new Exception("Set collection alias first");
            }

            Expression<Func<(IParent? parent, IEntity entity), IParent>> expression = ((IParent? parent, IEntity entity) combined) => new ParentEntity(combined.parent, combined.entity, CollectionAlias);

            RepositoryParentProperty = PropertyMetadataHelper.GetPropertyMetadata(expression);

            return this;
        }
    }
}

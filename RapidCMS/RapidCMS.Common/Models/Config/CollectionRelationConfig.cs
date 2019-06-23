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
    public class CollectionRelationConfig : RelationConfig
    {
        internal string CollectionAlias { get; set; }
        internal Type RelatedEntityType { get; set; }
        internal IPropertyMetadata? RepositoryParentIdProperty { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal IExpressionMetadata DisplayProperty { get; set; }

        internal Func<IEntity, IEnumerable<IRelatedElement>, IEnumerable<string>?>? ValidationFunction { get; set; }
    }

    public class CollectionRelationConfig<TEntity, TRelatedEntity> : CollectionRelationConfig
    {
        public CollectionRelationConfig()
        {
            RelatedEntityType = typeof(TRelatedEntity);
        }

        public CollectionRelationConfig<TEntity, TRelatedEntity> SetElementIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
        {
            IdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

            return this;
        }

        public CollectionRelationConfig<TEntity, TRelatedEntity> SetElementDisplayProperty(Expression<Func<TRelatedEntity, string>> propertyExpression)
        {
            DisplayProperty = PropertyMetadataHelper.GetExpressionMetadata(propertyExpression) ?? throw new InvalidExpressionException(nameof(propertyExpression));

            return this;
        }

        // HACK: hardcoded IEntity.Id type (string)
        public CollectionRelationConfig<TEntity, TRelatedEntity> SetRepositoryParentIdProperty(Expression<Func<TEntity, string>> propertyExpression)
        {
            RepositoryParentIdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

            return this;
        }

        public CollectionRelationConfig<TEntity, TRelatedEntity> ValidateRelation(Func<TEntity, IEnumerable<IRelatedElement>, IEnumerable<string>?> validationFunction)
        {
            ValidationFunction = (entity, relations) => (entity is TEntity correctEntity)
                ? validationFunction.Invoke(correctEntity, relations)
                : default;

            return this;
        }
    }
}

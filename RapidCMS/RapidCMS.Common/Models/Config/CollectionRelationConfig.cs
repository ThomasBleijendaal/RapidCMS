using System;
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
        internal IPropertyMetadata RepositoryParentIdProperty { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal IExpressionMetadata DisplayProperty { get; set; }
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

        public CollectionRelationConfig<TEntity, TRelatedEntity> SetRepositoryParentIdProperty<TValue>(Expression<Func<TEntity, TValue>> propertyExpression)
        {
            RepositoryParentIdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

            return this;
        }
    }
}

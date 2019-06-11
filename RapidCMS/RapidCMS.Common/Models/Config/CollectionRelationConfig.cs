using System;
using System.Linq.Expressions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;


namespace RapidCMS.Common.Models.Config
{
    public class CollectionRelationConfig : RelationConfig
    {
        internal string CollectionAlias { get; set; }
        internal Type RelatedEntityType { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal IExpressionMetadata DisplayProperty { get; set; }
    }

    public class CollectionRelationConfig<TEntity, TRelatedEntity> : CollectionRelationConfig
    {
        public CollectionRelationConfig()
        {
            RelatedEntityType = typeof(TRelatedEntity);
        }

        public CollectionRelationConfig<TEntity, TRelatedEntity> SetIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
        {
            IdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

            return this;
        }

        public CollectionRelationConfig<TEntity, TRelatedEntity> SetDisplayProperty(Expression<Func<TRelatedEntity, string>> propertyExpression)
        {
            DisplayProperty = PropertyMetadataHelper.GetExpressionMetadata(propertyExpression);

            return this;
        }
    }
}

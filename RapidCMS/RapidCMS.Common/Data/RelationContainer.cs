using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Helpers;

namespace RapidCMS.Common.Data
{
    internal class RelationContainer : IRelationContainer
    {
        public RelationContainer(IEnumerable<IRelation> relations)
        {
            Relations = relations ?? throw new ArgumentNullException(nameof(relations));
        }

        public IEnumerable<IRelation> Relations { get; private set; }

        public IEnumerable<TId> GetRelatedElementIdsFor<TEntity, TValue, TId>(Expression<Func<TEntity, TValue>> propertyExpression) where TEntity : IEntity
        {
            var property = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

            return Relations.FirstOrDefault(x => x.Property.Fingerprint == property.Fingerprint)?.RelatedElementIdsAs<TId>();
        }

        public IEnumerable<IRelatedElement> GetRelatedElementsFor<TEntity, TValue>(Expression<Func<TEntity, TValue>> propertyExpression) where TEntity : IEntity
        {
            var property = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

            return Relations.FirstOrDefault(x => x.Property.Fingerprint == property.Fingerprint)?.RelatedElements;
        }

        public IEnumerable<IRelatedElement> GetRelatedElementsFor<TRelatedEntity>() where TRelatedEntity : IEntity
        {
            return Relations.FirstOrDefault(x => x.RelatedEntity == typeof(TRelatedEntity))?.RelatedElements;
        }

        public IEnumerable<TId> GetRelatedElementIdsFor<TRelatedEntity, TId>() where TRelatedEntity : IEntity
        {
            return Relations.FirstOrDefault(x => x.RelatedEntity == typeof(TRelatedEntity))?.RelatedElementIdsAs<TId>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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

        public IReadOnlyList<TId> GetRelatedElementIdsFor<TEntity, TValue, TId>(Expression<Func<TEntity, TValue>> propertyExpression) where TEntity : IEntity
        {
            var property = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

            return Relations.FirstOrDefault(x => x.Property.Fingerprint == property?.Fingerprint)?.RelatedElementIdsAs<TId>() ?? new List<TId>();
        }

        public IReadOnlyList<IElement> GetRelatedElementsFor<TEntity, TValue>(Expression<Func<TEntity, TValue>> propertyExpression) where TEntity : IEntity
        {
            var property = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

            return Relations.FirstOrDefault(x => x.Property.Fingerprint == property?.Fingerprint)?.RelatedElements ?? new List<IElement>();
        }

        public IReadOnlyList<IElement> GetRelatedElementsFor<TRelatedEntity>() where TRelatedEntity : IEntity
        {
            return Relations.FirstOrDefault(x => x.RelatedEntity == typeof(TRelatedEntity))?.RelatedElements ?? new List<IElement>();
        }

        public IReadOnlyList<TId> GetRelatedElementIdsFor<TRelatedEntity, TId>() where TRelatedEntity : IEntity
        {
            return Relations.FirstOrDefault(x => x.RelatedEntity == typeof(TRelatedEntity))?.RelatedElementIdsAs<TId>() ?? new List<TId>();
        }
    }
}

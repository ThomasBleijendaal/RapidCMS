using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RapidCMS.Common.Data
{
    public interface IRelationContainer
    {
        IEnumerable<IRelation> Relations { get; }

        // TODO: convert to IReadOnlyList
        IEnumerable<IRelatedElement> GetRelatedElementsFor<TEntity, TValue>(Expression<Func<TEntity, TValue>> propertyExpression)
            where TEntity : IEntity;

        // TODO: convert to IReadOnlyList
        IEnumerable<TId> GetRelatedElementIdsFor<TEntity, TValue, TId>(Expression<Func<TEntity, TValue>> propertyExpression)
            where TEntity : IEntity;

        IEnumerable<IRelatedElement> GetRelatedElementsFor<TRelatedEntity>()
            where TRelatedEntity : IEntity;

        IEnumerable<TId> GetRelatedElementIdsFor<TRelatedEntity, TId>()
            where TRelatedEntity : IEntity;
    }
}

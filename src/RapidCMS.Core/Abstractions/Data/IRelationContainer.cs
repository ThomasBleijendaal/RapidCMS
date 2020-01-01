using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IRelationContainer
    {
        IEnumerable<IRelation> Relations { get; }

        IReadOnlyList<IElement>? GetRelatedElementsFor<TEntity, TValue>(Expression<Func<TEntity, TValue>> propertyExpression)
            where TEntity : IEntity;

        IReadOnlyList<TId>? GetRelatedElementIdsFor<TEntity, TValue, TId>(Expression<Func<TEntity, TValue>> propertyExpression)
            where TEntity : IEntity;

        IReadOnlyList<IElement>? GetRelatedElementsFor<TRelatedEntity>()
            where TRelatedEntity : IEntity;

        IReadOnlyList<TId>? GetRelatedElementIdsFor<TRelatedEntity, TId>()
            where TRelatedEntity : IEntity;
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Forms;

public interface IRelationContainer
{
    IEnumerable<IRelation> Relations { get; }

    IReadOnlyList<TId>? GetRelatedElementIdsFor<TEntity, TValue, TId>(Expression<Func<TEntity, TValue>> propertyExpression)
        where TEntity : IEntity;

    IReadOnlyList<TId>? GetRelatedElementIdsFor<TRelatedEntity, TId>()
        where TRelatedEntity : IEntity;
}

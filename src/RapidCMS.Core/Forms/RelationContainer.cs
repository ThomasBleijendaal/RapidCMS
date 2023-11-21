using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Forms;

internal class RelationContainer : IRelationContainer
{
    public RelationContainer(IEnumerable<IRelation> relations)
    {
        Relations = relations ?? throw new ArgumentNullException(nameof(relations));
    }

    public IEnumerable<IRelation> Relations { get; private set; }

    public IReadOnlyList<TId>? GetRelatedElementIdsFor<TEntity, TValue, TId>(Expression<Func<TEntity, TValue>> propertyExpression) where TEntity : IEntity
    {
        var property = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

        return Relations.FirstOrDefault(x => x.Property.Fingerprint == property?.Fingerprint)?.RelatedElementIdsAs<TId>() ?? default;
    }

    public IReadOnlyList<TId>? GetRelatedElementIdsFor<TRelatedEntity, TId>() where TRelatedEntity : IEntity
    {
        return Relations.FirstOrDefault(x => x.RelatedEntityType == typeof(TRelatedEntity))?.RelatedElementIdsAs<TId>() ?? default;
    }
}

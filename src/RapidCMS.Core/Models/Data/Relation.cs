using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Models.Data;

internal class Relation : IRelation
{
    public Relation(Type relatedEntity, IPropertyMetadata property, IReadOnlyList<object> relatedElementIds)
    {
        RelatedEntityType = relatedEntity ?? throw new ArgumentNullException(nameof(relatedEntity));
        Property = property ?? throw new ArgumentNullException(nameof(property));
        RelatedElementIds = relatedElementIds ?? throw new ArgumentNullException(nameof(relatedElementIds));
    }

    public Type RelatedEntityType { get; private set; }

    public IPropertyMetadata Property { get; private set; }

    public IReadOnlyList<object> RelatedElementIds { get; private set; }

    public IReadOnlyList<T> RelatedElementIdsAs<T>()
    {
        if (typeof(T) == typeof(int))
        {
            // edge case for getting int32 when list was deserialized to int64
            return RelatedElementIds.Select(Convert.ToInt32).OfType<T>().ToList();
        }
        else
        {
            return RelatedElementIds.OfType<T>().ToList();
        }
    }
}

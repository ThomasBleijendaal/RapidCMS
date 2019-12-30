using System;
using System.Collections.Generic;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Interfaces.Data
{
    public interface IRelation
    {
        Type RelatedEntity { get; }
        IPropertyMetadata Property { get; }
        IReadOnlyList<IElement> RelatedElements { get; }
        IReadOnlyList<T> RelatedElementIdsAs<T>();
    }
}

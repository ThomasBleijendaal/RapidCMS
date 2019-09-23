using System;
using System.Collections.Generic;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    public interface IRelation
    {
        Type RelatedEntity { get; }
        IPropertyMetadata Property { get; }
        IReadOnlyList<IElement> RelatedElements { get; }
        IReadOnlyList<T> RelatedElementIdsAs<T>();
    }
}

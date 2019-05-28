using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    internal class Relation : IRelation
    {
        public Relation(Type relatedEntity, IPropertyMetadata property, IEnumerable<IRelatedElement> relatedElements)
        {
            RelatedEntity = relatedEntity ?? throw new ArgumentNullException(nameof(relatedEntity));
            Property = property ?? throw new ArgumentNullException(nameof(property));
            RelatedElements = relatedElements ?? throw new ArgumentNullException(nameof(relatedElements));
        }

        public Type RelatedEntity { get; private set; }

        public IPropertyMetadata Property { get; private set; }

        // TODO: convert to IReadOnlyList
        public IEnumerable<IRelatedElement> RelatedElements { get; private set; }

        // TODO: convert to IReadOnlyList
        public IEnumerable<T> RelatedElementIdsAs<T>()
        {
            return RelatedElements.Select(x => x.Id).Cast<T>();
        }
    }
}

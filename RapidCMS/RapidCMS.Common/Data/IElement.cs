using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Data
{
    public interface IElement
    {
        object Id { get; }

        // TODO: make label columnable (support for multiple columns in UI)
        string Label { get; }
    }

    public interface IRelatedElement
    {
        object Id { get; }
    }

    internal class RelatedElement : IRelatedElement
    {
        public object Id { get; internal set; }
    }

    public interface IRelation
    {
        IPropertyMetadata Property { get; }
        IEnumerable<IRelatedElement> RelatedElements { get; }
        IEnumerable<T> RelatedElementIdsAs<T>();
    }

    // TODO: change RelatedElements to IOrderedEnumerable or something related
    internal class Relation : IRelation
    {
        public Relation(IPropertyMetadata property, IEnumerable<IRelatedElement> relatedElements)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            RelatedElements = relatedElements ?? throw new ArgumentNullException(nameof(relatedElements));
        }

        public IPropertyMetadata Property { get; private set; }

        public IEnumerable<IRelatedElement> RelatedElements { get; private set; }

        public IEnumerable<T> RelatedElementIdsAs<T>()
        {
            return RelatedElements.Select(x => x.Id).Cast<T>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;

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

    public interface IRelation
    {
        Type RelatedEntity { get; }
        IPropertyMetadata Property { get; }
        IEnumerable<IRelatedElement> RelatedElements { get; }
        IEnumerable<T> RelatedElementIdsAs<T>();
    }

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

    internal class RelationContainer : IRelationContainer
    {
        public RelationContainer(IEnumerable<IRelation> relations)
        {
            Relations = relations ?? throw new ArgumentNullException(nameof(relations));
        }

        public IEnumerable<IRelation> Relations { get; private set; }

        public IEnumerable<TId> GetRelatedElementIdsFor<TEntity, TValue, TId>(Expression<Func<TEntity, TValue>> propertyExpression) where TEntity : IEntity
        {
            var property = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

            return Relations.FirstOrDefault(x => x.Property.Fingerprint == property.Fingerprint)?.RelatedElementIdsAs<TId>();
        }

        public IEnumerable<IRelatedElement> GetRelatedElementsFor<TEntity, TValue>(Expression<Func<TEntity, TValue>> propertyExpression) where TEntity : IEntity
        {
            var property = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

            return Relations.FirstOrDefault(x => x.Property.Fingerprint == property.Fingerprint)?.RelatedElements;
        }

        public IEnumerable<IRelatedElement> GetRelatedElementsFor<TRelatedEntity>() where TRelatedEntity : IEntity
        {
            return Relations.FirstOrDefault(x => x.RelatedEntity == typeof(TRelatedEntity))?.RelatedElements;
        }

        public IEnumerable<TId> GetRelatedElementIdsFor<TRelatedEntity, TId>() where TRelatedEntity : IEntity
        {
            return Relations.FirstOrDefault(x => x.RelatedEntity == typeof(TRelatedEntity))?.RelatedElementIdsAs<TId>();
        }
    }
}

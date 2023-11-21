using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.Config;

internal class RepositoryRelationConfig : RelationConfig
{
    public RepositoryRelationConfig()
    {

    }

    public RepositoryRelationConfig(string? collectionAlias, bool isRelationToMany)
    {
        CollectionAlias = collectionAlias;
        IsRelationToMany = isRelationToMany;
    }

    protected internal string? CollectionAlias { get; private set; }
    protected internal Type? RepositoryType { get; protected set; }
    internal Type? RelatedEntityType { get; set; }
    protected internal Type? RelatedRepositoryType { get; protected set; }
    internal IPropertyMetadata? RelatedElementsGetter { get; set; }
    internal IPropertyMetadata? RepositoryParentProperty { get; set; }
    internal bool EntityAsParent { get; set; }
    internal IPropertyMetadata? IdProperty { get; set; }
    internal List<IExpressionMetadata>? DisplayProperties { get; set; }
    internal bool IsRelationToMany { get; set; }
}

internal class RepositoryRelationConfig<TEntity, TRelatedEntity> : RepositoryRelationConfig, ICollectionRelationConfig<TEntity, TRelatedEntity>
    where TEntity : IEntity
{
    public RepositoryRelationConfig(string collectionAlias, bool isRelationToMany) : base(collectionAlias, isRelationToMany)
    {
        RelatedEntityType = typeof(TRelatedEntity);
    }

    public RepositoryRelationConfig(Type relatedRepositoryType, bool isRelationToMany)
    {
        RepositoryType = relatedRepositoryType;
        RelatedEntityType = typeof(TRelatedEntity);
        RelatedRepositoryType = relatedRepositoryType;
        IsRelationToMany = isRelationToMany;
    }

    public RepositoryRelationConfig(string collectionAlias, IPropertyMetadata relatedElements) : base(collectionAlias, true)
    {
        RelatedEntityType = typeof(TRelatedEntity);
        RelatedElementsGetter = relatedElements;
    }

    public RepositoryRelationConfig(Type relatedRepositoryType, IPropertyMetadata relatedElements)
    {
        RepositoryType = relatedRepositoryType;
        RelatedEntityType = typeof(TRelatedEntity);
        RelatedRepositoryType = relatedRepositoryType;
        RelatedElementsGetter = relatedElements;
        IsRelationToMany = true;
    }

    public ICollectionRelationConfig<TEntity, TRelatedEntity> SetElementIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
    {
        IdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

        return this;
    }

    public ICollectionRelationConfig<TEntity, TRelatedEntity> SetElementDisplayProperties(params Expression<Func<TRelatedEntity, string?>>[] propertyExpressions)
    {
        DisplayProperties = propertyExpressions
            .Select(propertyExpression => PropertyMetadataHelper.GetExpressionMetadata(propertyExpression) ?? throw new InvalidExpressionException(nameof(propertyExpression)))
            .ToList();

        return this;
    }

    public ICollectionRelationConfig<TEntity, TRelatedEntity> SetRepositoryParent(Expression<Func<IParent, IParent?>> propertyExpression)
    {
        RepositoryParentProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression));

        return this;
    }

    public ICollectionRelationConfig<TEntity, TRelatedEntity> SetEntityAsParent()
    {
        EntityAsParent = true;
        RepositoryParentProperty = null;

        return this;
    }
}

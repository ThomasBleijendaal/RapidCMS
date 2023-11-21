using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Models.Setup;

public class RepositoryRelationSetup : RelationSetup
{
    public RepositoryRelationSetup(
        string? repositoryAlias,
        string? collectionAlias,
        Type? relatedEntityType,
        IPropertyMetadata? idProperty,
        List<IExpressionMetadata>? displayProperties,
        bool isRelationToMany)
    {
        CollectionAlias = collectionAlias;
        RepositoryAlias = repositoryAlias;
        RelatedEntityType = relatedEntityType;
        IdProperty = idProperty;
        DisplayProperties = displayProperties;
        IsRelationToMany = isRelationToMany;
    }

    public string? CollectionAlias { get; set; }
    public string? RepositoryAlias { get; set; }
    public Type? RelatedEntityType { get; set; }
    public IPropertyMetadata? RelatedElementsGetter { get; set; }
    public IPropertyMetadata? RepositoryParentSelector { get; set; }
    public bool EntityAsParent { get; set; }
    public IPropertyMetadata? IdProperty { get; set; }
    public List<IExpressionMetadata>? DisplayProperties { get; set; }
    public bool IsRelationToMany { get; }
}

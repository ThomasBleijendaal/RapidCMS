using System;

namespace RapidCMS.Core.Models.Config;

internal class RelationDataProviderRelationConfig : RelationConfig
{
    internal RelationDataProviderRelationConfig(Type relationDataCollectionType, object? configuration)
    {
        RelationDataCollectionType = relationDataCollectionType ?? throw new ArgumentNullException(nameof(relationDataCollectionType));
        Configuration = configuration;
    }

    public Type RelationDataCollectionType { get; }
    public object? Configuration { get; }
}

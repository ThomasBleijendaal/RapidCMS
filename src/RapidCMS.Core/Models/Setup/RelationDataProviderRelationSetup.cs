using System;

namespace RapidCMS.Core.Models.Setup
{
    public class RelationDataProviderRelationSetup : RelationSetup
    {
        public RelationDataProviderRelationSetup(Type relationDataCollectionType, object? configuration)
        {
            RelationDataCollectionType = relationDataCollectionType ?? throw new ArgumentNullException(nameof(relationDataCollectionType));
            Configuration = configuration;
        }

        public Type RelationDataCollectionType { get; }
        public object? Configuration { get; }
    }
}

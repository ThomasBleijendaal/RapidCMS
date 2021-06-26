using System;

namespace RapidCMS.Core.Models.Setup
{
    public class DataProviderRelationSetup : RelationSetup
    {
        public DataProviderRelationSetup(Type dataCollectionType, object? configuration)
        {
            DataCollectionType = dataCollectionType ?? throw new ArgumentNullException(nameof(dataCollectionType));
            Configuration = configuration;
        }

        public Type DataCollectionType { get; }
        public object? Configuration { get; }
    }
}

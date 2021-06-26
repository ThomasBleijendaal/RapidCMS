using System;

namespace RapidCMS.Core.Models.Config
{
    internal class DataProviderRelationConfig : RelationConfig
    {
        internal DataProviderRelationConfig(Type dataCollectionType, object? configuration)
        {
            DataCollectionType = dataCollectionType ?? throw new ArgumentNullException(nameof(dataCollectionType));
            Configuration = configuration;
        }

        public Type DataCollectionType { get; }
        public object? Configuration { get; }
    }
}

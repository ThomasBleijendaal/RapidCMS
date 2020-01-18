using System;

namespace RapidCMS.Core.Models.Config
{
    internal class DataProviderRelationConfig : RelationConfig
    {
        internal DataProviderRelationConfig(Type dataCollectionType)
        {
            DataCollectionType = dataCollectionType ?? throw new ArgumentNullException(nameof(dataCollectionType));
        }

        internal Type DataCollectionType { get; set; }
    }
}

using System;

namespace RapidCMS.Common.Models.Config
{
    public class DataProviderRelationConfig : RelationConfig
    {
        public DataProviderRelationConfig(Type dataCollectionType)
        {
            DataCollectionType = dataCollectionType ?? throw new ArgumentNullException(nameof(dataCollectionType));
        }

        internal Type DataCollectionType { get; set; }
    }
}

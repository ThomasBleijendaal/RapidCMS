using System;

namespace RapidCMS.Core.Models.Setup
{
    public class DataProviderRelationSetup : RelationSetup
    {
        public DataProviderRelationSetup(Type dataCollectionType)
        {
            DataCollectionType = dataCollectionType ?? throw new ArgumentNullException(nameof(dataCollectionType));
        }

        public Type DataCollectionType { get; set; }
    }
}

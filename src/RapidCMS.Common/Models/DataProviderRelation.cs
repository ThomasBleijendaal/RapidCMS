using System;


namespace RapidCMS.Common.Models
{
    internal class DataProviderRelation : Relation
    {
        public DataProviderRelation(Type dataCollectionType)
        {
            DataCollectionType = dataCollectionType ?? throw new ArgumentNullException(nameof(dataCollectionType));
        }

        internal Type DataCollectionType { get; set; }
    }
}

using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Setup
{
    public class ConcreteDataProviderRelationSetup : RelationSetup
    {
        public ConcreteDataProviderRelationSetup(IDataCollection dataCollection)
        {
            DataCollection = dataCollection ?? throw new ArgumentNullException(nameof(dataCollection));
        }

        public IDataCollection DataCollection { get; set; }
    }
}

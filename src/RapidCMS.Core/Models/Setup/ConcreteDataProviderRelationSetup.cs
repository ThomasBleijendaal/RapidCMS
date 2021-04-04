using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Setup
{
    // TODO: expose this class also via Config > Setup route
    internal class ConcreteDataProviderRelationSetup : RelationSetup
    {
        public ConcreteDataProviderRelationSetup(IDataCollection dataCollection)
        {
            DataCollection = dataCollection ?? throw new ArgumentNullException(nameof(dataCollection));
        }

        internal IDataCollection DataCollection { get; set; }
    }
}

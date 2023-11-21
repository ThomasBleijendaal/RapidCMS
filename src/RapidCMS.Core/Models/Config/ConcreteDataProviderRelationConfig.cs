using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Config;

internal class ConcreteDataProviderRelationConfig : RelationConfig
{
    internal ConcreteDataProviderRelationConfig(IDataCollection dataCollection)
    {
        DataCollection = dataCollection ?? throw new ArgumentNullException(nameof(dataCollection));
    }

    internal IDataCollection DataCollection { get; set; }
}

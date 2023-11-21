using System;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup;

public class SubCollectionListSetup
{
    public SubCollectionListSetup(int index, string collectionAlias)
    {
        Index = index;
        CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
    }

    public int Index { get; set; }
    public string CollectionAlias { get; set; }

    public UsageType SupportsUsageType { get; set; }
}

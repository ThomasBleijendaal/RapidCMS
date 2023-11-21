using System;

namespace RapidCMS.Core.Models.Config;

internal class CollectionListConfig
{
    internal CollectionListConfig(string collectionAlias)
    {
        CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
    }

    internal int Index { get; set; }

    internal string CollectionAlias { get; set; }

    internal Type? RepositoryType { get; set; }
    internal Type? EntityType { get; set; }
    internal ListConfig? ListEditor { get; set; }
    internal ListConfig? ListView { get; set; }
}

using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Plugins
{
    internal interface IPlugin
    {
        IEnumerable<ITreeElementSetup> GetTreeElements();

        CollectionSetup? GetCollection(string collectionAlias);

        Type? GetRepository(string collectionAlias);

        string CollectionPrefix { get; }
    }
}

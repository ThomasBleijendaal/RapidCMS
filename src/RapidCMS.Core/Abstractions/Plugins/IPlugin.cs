using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Plugins
{
    internal interface IPlugin
    {
        IEnumerable<ITreeElementSetup> GetTreeElements();

        CollectionSetup? GetCollection(string collectionAlias);

        Type? GetRepositoryType(string collectionAlias);

        string CollectionPrefix { get; }
    }
}

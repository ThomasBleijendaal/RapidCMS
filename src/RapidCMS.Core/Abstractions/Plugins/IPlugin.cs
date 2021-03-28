using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Plugins
{
    internal interface IPlugin
    {
        Task<IEnumerable<ITreeElementSetup>> GetTreeElementsAsync();

        Task<CollectionSetup?> GetCollectionAsync(string collectionAlias);

        Type? GetRepositoryType(string collectionAlias);

        string CollectionPrefix { get; }
    }
}

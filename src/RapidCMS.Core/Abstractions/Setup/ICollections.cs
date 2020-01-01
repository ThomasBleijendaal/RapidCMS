using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface ICollections
    {
        CollectionSetup GetCollection(string alias);
        IEnumerable<CollectionSetup> GetRootCollections();
    }

    internal interface IRepositories
    {
        IRepository GetRepository(CollectionSetup collection);
        IRepository GetRepository(string collectionAlias);
    }
}

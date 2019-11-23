using System.Collections.Generic;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Providers
{
    internal interface ICollectionProvider
    {
        Collection GetCollection(string alias);
        IEnumerable<Collection> GetAllCollections();
        IRepository? GetRepository(string collectionAlias);
    }
}

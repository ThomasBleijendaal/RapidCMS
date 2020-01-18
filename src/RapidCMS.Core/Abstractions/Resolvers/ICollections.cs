using System.Collections.Generic;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface ICollectionResolver
    {
        CollectionSetup GetCollection(string alias);
        IEnumerable<CollectionSetup> GetRootCollections();
    }
}

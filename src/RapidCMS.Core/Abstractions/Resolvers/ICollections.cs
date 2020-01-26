using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface ICollectionResolver
    {
        ICollectionSetup GetCollection(string alias);
        IEnumerable<ICollectionSetup> GetRootCollections();
    }
}

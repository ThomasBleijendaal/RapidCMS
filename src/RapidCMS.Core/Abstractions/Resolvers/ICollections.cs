using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    [Obsolete("This can be via a ISetupResolver")]
    internal interface ICollectionResolver
    {
        ICollectionSetup GetCollection(string alias);
        IPageSetup GetPage(string alias);
        IEnumerable<ITreeElementSetup> GetRootCollections();
    }
}

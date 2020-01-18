using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IRepositoryResolver
    {
        IRepository GetRepository(CollectionSetup collection);
        IRepository GetRepository(string collectionAlias);
    }
}

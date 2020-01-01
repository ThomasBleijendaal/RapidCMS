using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers
{
    internal interface IRepositoryResolver
    {
        IRepository GetRepository(CollectionSetup collection);
        IRepository GetRepository(string collectionAlias);
    }
}

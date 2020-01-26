using System;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface IRepositoryResolver
    {
        internal IRepository GetRepository(ICollectionSetup collection);
        public IRepository GetRepository(string collectionAlias);
        public IRepository GetRepository(Type repositoryType);
    }
}

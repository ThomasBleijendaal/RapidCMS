using System;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface IRepositoryResolver
    {
        internal IRepository GetRepository(ICollectionSetup collection);
        public IRepository GetRepository(string repositoryAlias);
        public IRepository GetRepository(Type repositoryType);
    }
}

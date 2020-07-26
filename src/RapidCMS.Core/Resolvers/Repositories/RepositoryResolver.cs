using System;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Resolvers.Repositories
{
    internal class RepositoryResolver : IRepositoryResolver
    {
        private readonly IRepositoryTypeResolver _repositoryTypeResolver;
        private readonly IServiceProvider _serviceProvider;

        public RepositoryResolver(IRepositoryTypeResolver repositoryTypeResolver, IServiceProvider serviceProvider)
        {
            _repositoryTypeResolver = repositoryTypeResolver;
            _serviceProvider = serviceProvider;
        }

        IRepository IRepositoryResolver.GetRepository(ICollectionSetup collection)
            => (this as IRepositoryResolver).GetRepository(collection.RepositoryAlias);

        IRepository IRepositoryResolver.GetRepository(string repositoryAlias)
            => (this as IRepositoryResolver).GetRepository(_repositoryTypeResolver.GetType(repositoryAlias));

        IRepository IRepositoryResolver.GetRepository(Type repositoryType) 
            => (IRepository)_serviceProvider.GetRequiredService(repositoryType);
    }
}

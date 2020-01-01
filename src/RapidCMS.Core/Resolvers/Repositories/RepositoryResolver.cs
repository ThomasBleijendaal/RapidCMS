using System;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Repositories
{
    internal class RepositoryResolver : IRepositoryResolver
    {
        private readonly ICollections _collections;
        private readonly IServiceProvider _serviceProvider;

        public RepositoryResolver(ICollections collections, IServiceProvider serviceProvider)
        {
            _collections = collections;
            _serviceProvider = serviceProvider;
        }

        public IRepository GetRepository(CollectionSetup collection)
        {
            return (IRepository)_serviceProvider.GetRequiredService(collection.RepositoryType);
        }

        public IRepository GetRepository(string collectionAlias)
        {
            return GetRepository(_collections.GetCollection(collectionAlias));
        }
    }
}

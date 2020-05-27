using System;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Resolvers.Repositories
{
    internal class ApiRepositoryResolver : IRepositoryResolver
    {
        private readonly ApiConfig _apiConfig;
        private readonly IServiceProvider _serviceProvider;

        public ApiRepositoryResolver(ApiConfig apiConfig, IServiceProvider serviceProvider)
        {
            _apiConfig = apiConfig;
            _serviceProvider = serviceProvider;
        }

        IRepository IRepositoryResolver.GetRepository(ICollectionSetup collection)
        {
            return (IRepository)_serviceProvider.GetRequiredService(collection.RepositoryType);
        }

        IRepository IRepositoryResolver.GetRepository(string collectionAlias)
        {
            if (_apiConfig.Collections.TryGetValue(collectionAlias, out var collection))
            {
                return (this as IRepositoryResolver).GetRepository(collection.RepositoryType);
            }
             
            throw new InvalidOperationException($"Collection with alias {collectionAlias} not registered.");
        }

        IRepository IRepositoryResolver.GetRepository(Type repositoryType)
        {
            return (IRepository)_serviceProvider.GetRequiredService(repositoryType);
        }
    }
}

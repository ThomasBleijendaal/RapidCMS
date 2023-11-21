using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Repositories;

internal class RepositoryResolver : IRepositoryResolver
{
    private readonly IRepositoryTypeResolver _repositoryTypeResolver;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IPlugin> _plugins;

    public RepositoryResolver(
        IRepositoryTypeResolver repositoryTypeResolver, 
        IServiceProvider serviceProvider,
        IEnumerable<IPlugin> plugins)
    {
        _repositoryTypeResolver = repositoryTypeResolver;
        _serviceProvider = serviceProvider;
        _plugins = plugins;
    }

    IRepository IRepositoryResolver.GetRepository(CollectionSetup collection)
        => (this as IRepositoryResolver).GetRepository(collection.RepositoryAlias);

    IRepository IRepositoryResolver.GetRepository(string repositoryAlias)
    {
        if (repositoryAlias.TryParseAsPluginAlias(out var alias) && _plugins.FirstOrDefault(x => x.CollectionPrefix == alias.prefix) is IPlugin plugin)
        {
            return (this as IRepositoryResolver).GetRepository(plugin.GetRepositoryType(alias.collectionAlias) 
                ?? throw new InvalidOperationException($"Cannot resolve plugin repository type with alias {repositoryAlias}."));
        }

        return (this as IRepositoryResolver).GetRepository(_repositoryTypeResolver.GetType(repositoryAlias));
    }

    IRepository IRepositoryResolver.GetRepository(Type repositoryType)
        => (IRepository)_serviceProvider.GetRequiredService(repositoryType);
}

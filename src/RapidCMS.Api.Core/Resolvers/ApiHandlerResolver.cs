using System;
using System.Linq;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Core.Handlers;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Api.Core.Resolvers;

internal class ApiHandlerResolver : IApiHandlerResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IApiConfig _config;

    public ApiHandlerResolver(
        IServiceProvider serviceProvider,
        IApiConfig config)
    {
        _serviceProvider = serviceProvider;
        _config = config;
    }

    public IApiHandler GetApiHandler(string repositoryAlias)
    {
        var repoConfig = _config.Repositories.FirstOrDefault(x => x.Alias == repositoryAlias);
        if (repoConfig == null)
        {
            throw new InvalidOperationException($"Could not find repository with alias {repositoryAlias}. Available: {string.Join(",", _config.Repositories.Select(x => $"[{x.ApiRepositoryType}: {x.Alias}]"))}.");
        }

        Type repoType;
        if (repoConfig.DatabaseType == default)
        {
            repoType = typeof(ApiHandler<,,>)
                .MakeGenericType(repoConfig.EntityType, repoConfig.EntityType, repoConfig.ApiRepositoryType);
        }
        else
        {
            repoType = typeof(ApiHandler<,,>)
                .MakeGenericType(repoConfig.EntityType, repoConfig.DatabaseType, repoConfig.ApiRepositoryType);
        }

        return _serviceProvider.GetService<IApiHandler>(repoType);
    }
}

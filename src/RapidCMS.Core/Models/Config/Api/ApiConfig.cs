using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Validators;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Repositories.ApiBridge;

namespace RapidCMS.Core.Models.Config.Api;

internal class ApiConfig : IApiConfig
{
    internal ApiAdvancedConfig AdvancedConfig { get; set; } = new ApiAdvancedConfig();

    internal bool AllowAnonymousUsage { get; set; } = false;
    internal List<ApiRepositoryConfig> Repositories { get; set; } = new List<ApiRepositoryConfig>();
    internal List<IApiDataViewBuilderConfig> DataViews { get; set; } = new List<IApiDataViewBuilderConfig>();
    internal List<FileUploadHandlerConfig> FileUploadHandlers { get; set; } = new List<FileUploadHandlerConfig>();
    internal List<(string entity, ValidationConfig validation)> EntityValidationConfig { get; set; } = new List<(string, ValidationConfig)>();

    IEnumerable<IApiRepositoryConfig> IApiConfig.Repositories => Repositories;
    IEnumerable<IApiDataViewBuilderConfig> IApiConfig.DataViews => DataViews;
    IEnumerable<IFileUploadHandlerConfig> IApiConfig.FileUploadHandlers => FileUploadHandlers;

    public IAdvancedApiConfig Advanced => AdvancedConfig;

    public IApiConfig AllowAnonymousUser()
    {
        AllowAnonymousUsage = true;
        return this;
    }

    public IApiConfig RegisterFileUploadHandler<THandler>() where THandler : IFileUploadHandler
    {
        FileUploadHandlers.Add(new FileUploadHandlerConfig
        {
            Alias = AliasHelper.GetFileUploaderAlias(typeof(THandler)),
            HandlerType = typeof(THandler)
        });

        return this;
    }

    public IApiRepositoryConfig RegisterRepository<TEntity, TRepository>()
        where TEntity : class, IEntity
        where TRepository : IRepository
    {
        var fullType = typeof(ApiRepository<TEntity>);
        var alias = AliasHelper.GetRepositoryAlias(fullType);

        if (Repositories.Any(x => x.Alias == alias))
        {
            throw new NotUniqueException(nameof(TRepository));
        }

        var config = new ApiRepositoryConfig
        {
            Alias = alias,
            EntityType = typeof(TEntity),
            RepositoryType = typeof(TRepository),
            ApiRepositoryType = fullType
        };
        Repositories.Add(config);
        return config;
    }

    public IApiRepositoryConfig RegisterRepository<TEntity, TMappedEntity, TRepository>()
        where TEntity : class, IEntity
        where TMappedEntity : class
        where TRepository : IRepository
    {
        var fullType = typeof(ApiMappedRepository<TEntity, TMappedEntity>);
        var alias = AliasHelper.GetRepositoryAlias(fullType);

        if (Repositories.Any(x => x.Alias == alias))
        {
            throw new NotUniqueException(nameof(TRepository));
        }

        var config = new ApiRepositoryConfig
        {
            Alias = alias,
            EntityType = typeof(TEntity),
            DatabaseType = typeof(TMappedEntity),
            RepositoryType = typeof(TRepository),
            ApiRepositoryType = fullType
        };
        Repositories.Add(config);
        return config;
    }

    public IApiConfig RegisterDataViewBuilder<TDataViewBuilder>(string collectionAlias)
         where TDataViewBuilder : IDataViewBuilder
    {
        if (DataViews.Any(x => x.Alias == collectionAlias))
        {
            throw new NotUniqueException(nameof(collectionAlias));
        }

        DataViews.Add(new ApiDataViewBuilderConfig
        {
            Alias = collectionAlias,
            DataViewBuilder = typeof(TDataViewBuilder)
        });

        return this;
    }

    public IApiConfig RegisterEntityValidator<TEntity, TEntityValidator>(object? config = default)
        where TEntity : IEntity
        where TEntityValidator : IEntityValidator
    {
        EntityValidationConfig.Add((AliasHelper.GetEntityVariantAlias(typeof(TEntity)), new ValidationConfig(typeof(TEntityValidator), config)));

        foreach (var subType in typeof(TEntity).GetSubTypes())
        {
            EntityValidationConfig.Add((AliasHelper.GetEntityVariantAlias(subType), new ValidationConfig(typeof(TEntityValidator), config)));
        }

        return this;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Core.Handlers;
using RapidCMS.Api.Core.Resolvers;
using RapidCMS.Api.Core.Services;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Dispatchers.Api;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Factories;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Mediators;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Config.Api;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Resolvers.Repositories;
using RapidCMS.Core.Resolvers.Setup;
using RapidCMS.Core.Services.Concurrency;
using RapidCMS.Core.Services.Parent;
using RapidCMS.Core.Services.Persistence;
using RapidCMS.Core.Services.Presentation;
using RapidCMS.Core.Validators;

namespace RapidCMS.Api.Core;

internal static class RapidCMSApiMiddlewareBase
{
    public static IServiceCollection AddRapidCMSApiCore(this IServiceCollection services, ApiConfig config)
    {
        services.AddSingleton<IApiConfig>(config);
        services.AddTransient<IApiHandlerResolver, ApiHandlerResolver>();
        services.AddTransient<IFileHandlerResolver, FileHandlerResolver>();

        //services.AddHttpContextAccessor();

        if (config.AllowAnonymousUsage)
        {
            services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
            services.AddSingleton<IUserResolver, AnonymousUserResolver>();
        }

        services.AddSingleton<ISetupResolver<EntityVariantSetup>, GlobalEntityVariantSetupResolver>();

        services.AddTransient<IDataViewResolver, ApiDataViewResolver>();
        services.AddTransient<IRepositoryResolver, RepositoryResolver>();
        services.AddSingleton<IRepositoryTypeResolver, ApiRepositoryTypeResolver>();

        services.AddTransient<IPresentationDispatcher, GetEntityDispatcher>();
        services.AddTransient<IPresentationDispatcher, GetEntitiesDispatcher>();
        services.AddTransient<IPresentationService, PresentationService>();

        services.AddTransient<IInteractionDispatcher, RelateEntityDispatcher>();
        services.AddTransient<IInteractionDispatcher, ReorderEntityDispatcher>();
        services.AddTransient<IInteractionDispatcher, PersistEntityDispatcher>();
        services.AddTransient<IInteractionDispatcher, DeleteEntityDispatcher>();
        services.AddTransient<IInteractionService, InteractionService>();

        services.AddTransient<IAuthService, ApiAuthService>();
        services.AddTransient<IParentService, ParentService>();
        services.AddTransient<IConcurrencyService, ConcurrencyService>();
        services.AddSingleton(serviceProvider => new SemaphoreSlim(5, 5));

        services.AddScoped<IMediator, Mediator>();

        services.AddTransient<IEditContextFactory, ApiEditContextWrapperFactory>();

        var apiHandlers = config.Repositories.ToList(
            repository => repository.DatabaseType == default
                ? typeof(ApiHandler<,,>).MakeGenericType(repository.EntityType, repository.EntityType, repository.ApiRepositoryType)
                : typeof(ApiHandler<,,>).MakeGenericType(repository.EntityType, repository.DatabaseType, repository.ApiRepositoryType));

        foreach (var apiHandler in apiHandlers)
        {
            services.AddTransient(apiHandler);
        }

        var entityVariants = config.Repositories.ToDictionary(x => x.Alias, x =>
        {
            var entityTypes = new[] { x.EntityType }
                .Union(x.EntityType.GetSubTypes())
                .ToList();
            return (entityType: x.EntityType, variants: (IReadOnlyList<Type>)entityTypes);
        });

        services.AddSingleton<IEntityVariantResolver>(new EntityVariantResolver(entityVariants));

        foreach (var fileHandler in config.FileUploadHandlers.ToList(fileHandler => typeof(FileHandler<>).MakeGenericType(fileHandler.HandlerType)))
        {
            services.AddTransient(fileHandler);
        }
        if (!config.AdvancedConfig.RemoveDataAnnotationEntityValidator)
        {
            services.AddSingleton<DataAnnotationEntityValidator>();

            foreach (var variant in entityVariants.SelectMany(x => x.Value.variants))
            {
                config.EntityValidationConfig.Add((AliasHelper.GetEntityVariantAlias(variant), new ValidationConfig(typeof(DataAnnotationEntityValidator), default)));
            }
        }

        var validations = config.EntityValidationConfig
            .GroupBy(x => x.entity)
            .ToDictionary(x => x.Key, x => x.ToList(x => new ValidationSetup(x.validation.Type, x.validation.Configuration) as ValidationSetup) as IReadOnlyList<ValidationSetup>);

        services.AddSingleton<ISetupResolver<IReadOnlyList<ValidationSetup>>>(new ValidationSetupResolver(validations));

        return services;
    }
}

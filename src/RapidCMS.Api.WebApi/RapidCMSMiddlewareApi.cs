using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using RapidCMS.Api.WebApi.Controllers;
using RapidCMS.Api.WebApi.Conventions;
using RapidCMS.Api.WebApi.Providers;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Converters;
using RapidCMS.Core.Dispatchers.Api;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Factories;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Mediators;
using RapidCMS.Core.Models.Config.Api;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Resolvers.Repositories;
using RapidCMS.Core.Resolvers.Setup;
using RapidCMS.Core.Services.Auth;
using RapidCMS.Core.Services.Parent;
using RapidCMS.Core.Services.Persistence;
using RapidCMS.Core.Services.Presentation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RapidCMSMiddlewareApi
    {
        private static IControllerModelConvention? _routeConvention;
        private static IApplicationFeatureProvider<ControllerFeature>? _controllerFeatureProvider;
        private static ApiConfig? _rootConfig;

        private static bool _controllersInstalled;

        /// <summary>
        /// Use this method to setup the Repository APIs to support RapidCMS WebAssenbly on a separate server.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddRapidCMSApi(this IServiceCollection services, Action<IApiConfig>? config = null)
        {
            if (_routeConvention != null || _controllerFeatureProvider != null)
            {
                throw new InvalidOperationException("Cannot call AddRapidCMSApi twice.");
            }

            _rootConfig = GetRootConfig(config);

            services.AddSingleton<IApiConfig>(_rootConfig);

            services.AddHttpContextAccessor();

            if (_rootConfig.AllowAnonymousUsage)
            {
                services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
                services.AddSingleton<AuthenticationStateProvider, AnonymousAuthenticationStateProvider>();
            }
            else
            {
                services.AddSingleton<AuthenticationStateProvider, HttpContextAuthenticationStateProvider>();
            }

            services.AddSingleton<ISetupResolver<IEntityVariantSetup>, GlobalEntityVariantSetupResolver>();

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

            services.AddScoped<IMediator, Mediator>();

            services.AddTransient<IEditContextFactory, ApiEditContextWrapperFactory>();

            var controllersToAdd = _rootConfig.Repositories.ToDictionary(
                repository =>
                {
                    if (repository.DatabaseType == default)
                    {
                        return typeof(ApiRepositoryController<,,>)
                            .MakeGenericType(repository.EntityType, repository.EntityType, repository.RepositoryType)
                            .GetTypeInfo();
                    }
                    else
                    {
                        return typeof(ApiRepositoryController<,,>)
                            .MakeGenericType(repository.EntityType, repository.DatabaseType, repository.RepositoryType)
                            .GetTypeInfo();
                    }
                },
                kv => kv.Alias);

            var entityVariants = _rootConfig.Repositories.ToDictionary(x => x.Alias, x =>
            {
                var entityTypes = new[] { x.EntityType }
                    .Union(x.EntityType.Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && t.IsSubclassOf(x.EntityType)))
                    .ToList();
                return (x.EntityType, (IReadOnlyList<Type>)entityTypes);
            });

            services.AddSingleton<IEntityVariantResolver>(new EntityVariantResolver(entityVariants));

            if (_rootConfig.FileUploadHandlers.Any())
            {
                foreach (var handler in _rootConfig.FileUploadHandlers)
                {
                    var type = typeof(ApiFileUploadController<>).MakeGenericType(handler).GetTypeInfo();
                    var alias = AliasHelper.GetFileUploaderAlias(type);

                    controllersToAdd.Add(type, alias);
                }
            }

            _controllerFeatureProvider = new CollectionControllerFeatureProvider(controllersToAdd.Keys);
            _routeConvention = new CollectionControllerRouteConvention(controllersToAdd);

            return services;
        }

        public static IMvcBuilder AddRapidCMSControllers(this IServiceCollection services, Action<MvcOptions>? extraConfig = default)
        {
            if (_rootConfig == null || _routeConvention == null || _controllerFeatureProvider == null)
            {
                throw new InvalidOperationException("Call AddRapidCMSApi() before calling this method.");
            }

            var builder = services.AddControllers(config =>
            {
                config.Conventions.Add(_routeConvention);

                extraConfig?.Invoke(config);
            }).AddNewtonsoftJson(options =>
            {
                foreach (var entityType in _rootConfig.Repositories.Select(x => x.EntityType))
                {
                    if (Activator.CreateInstance(typeof(EntityModelJsonConverter<>).MakeGenericType(entityType)) is JsonConverter jsonConverter)
                    {
                        options.SerializerSettings.Converters.Add(jsonConverter);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Could not create {nameof(EntityModelJsonConverter<IEntity>)} for {entityType.Name}");
                    } 
                }
            }).ConfigureApplicationPartManager(config =>
            {
                config.FeatureProviders.Add(_controllerFeatureProvider);
            });

            _controllersInstalled = true;

            return builder;
        }

        [Obsolete("Use AddRapidCMSControllers.")]
        public static void AddRapidCMSRouteConvention(this IList<IApplicationModelConvention> list)
        {
            if (_routeConvention == null)
            {
                throw new InvalidOperationException("Call AddRapidCMSApi() before calling this method.");
            }

            list.Add(_routeConvention);
        }

        [Obsolete("Use AddRapidCMSControllers.")]
        public static void AddRapidCMSControllerFeatureProvider(this IList<IApplicationFeatureProvider> list)
        {
            if (_controllerFeatureProvider == null)
            {
                throw new InvalidOperationException("Call AddRapidCMSApi() before calling this method.");
            }

            list.Add(_controllerFeatureProvider);
        }

        private static ApiConfig GetRootConfig(Action<IApiConfig>? config = null)
        {
            var rootConfig = new ApiConfig();
            config?.Invoke(rootConfig);
            return rootConfig;
        }

        public static IApplicationBuilder UseRapidCMSApi(this IApplicationBuilder app, bool isDevelopment = false)
        {
            if (!_controllersInstalled)
            {
                throw new InvalidOperationException($"Configure the convention and feature provider correctly first, using {nameof(AddRapidCMSControllers)}().");
            }

            return app;
        }
    }
}

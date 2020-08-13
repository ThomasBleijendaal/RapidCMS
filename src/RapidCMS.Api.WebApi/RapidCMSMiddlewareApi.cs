using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using RapidCMS.Api.WebApi.Controllers;
using RapidCMS.Api.WebApi.Conventions;
using RapidCMS.Api.WebApi.Providers;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Dispatchers.Api;
using RapidCMS.Core.Factories;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config.Api;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Resolvers.Repositories;
using RapidCMS.Core.Resolvers.Setup;
using RapidCMS.Core.Services.Auth;
using RapidCMS.Core.Services.Exceptions;
using RapidCMS.Core.Services.Parent;
using RapidCMS.Core.Services.Persistence;
using RapidCMS.Core.Services.Presentation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RapidCMSMiddlewareApi
    {
        private static IControllerModelConvention? _routeConvention;
        private static IApplicationFeatureProvider<ControllerFeature>? _controllerFeatureProvider;

        private static bool _routeConventionInstalled = false;
        private static bool _controlerFeatureInstalled = false;

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

            var rootConfig = GetRootConfig(config);

            services.AddSingleton<IApiConfig>(rootConfig);

            services.AddHttpContextAccessor();

            if (rootConfig.AllowAnonymousUsage)
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
            services.AddSingleton<IExceptionService, ExceptionService>();
            // TODO: message service
            services.AddTransient<IParentService, ParentService>();

            services.AddTransient<IEditContextFactory, ApiEditContextWrapperFactory>();

            var controllersToAdd = rootConfig.Repositories.ToDictionary(
                repository =>
                {
                    if (repository.DatabaseType == default)
                    {
                        return typeof(ApiRepositoryController<,>)
                            .MakeGenericType(repository.EntityType, repository.RepositoryType)
                            .GetTypeInfo();
                    }
                    else
                    {
                        return typeof(MappedApiRepositoryController<,,>)
                            .MakeGenericType(repository.EntityType, repository.DatabaseType, repository.RepositoryType)
                            .GetTypeInfo();
                    }
                },
                kv => kv.Alias);

            if (rootConfig.FileUploadHandlers.Any())
            {
                foreach (var handler in rootConfig.FileUploadHandlers)
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

        public static void AddRapidCMSRouteConvention(this IList<IApplicationModelConvention> list)
        {
            _routeConventionInstalled = true;

            if (_routeConvention == null)
            {
                throw new InvalidOperationException("Call AddRapidCMSApi() before calling this method.");
            }

            list.Add(_routeConvention);
        }

        public static void AddRapidCMSControllerFeatureProvider(this IList<IApplicationFeatureProvider> list)
        {
            _controlerFeatureInstalled = true;

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
            if (!_routeConventionInstalled || !_controlerFeatureInstalled)
            {
                throw new InvalidOperationException($"Configure the convention and feature provider correctly first, using {nameof(AddRapidCMSRouteConvention)}() and ({nameof(AddRapidCMSControllerFeatureProvider)}().");
            }

            return app;
        }
    }
}

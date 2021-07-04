using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;
using RapidCMS.Api.Core;
using RapidCMS.Api.WebApi.Conventions;
using RapidCMS.Api.WebApi.Resolvers;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Converters;
using RapidCMS.Core.Models.Config.Api;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RapidCMSMiddlewareWebApi
    {
        private static IControllerModelConvention? _routeConvention;
        private static ApiConfig? _rootConfig;

        /// <summary>
        /// Use this method to setup the Repository APIs to support RapidCMS WebAssenbly on a separate server.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddRapidCMSWebApi(this IServiceCollection services, Action<IApiConfig>? config = null)
        {
            _rootConfig = GetRootConfig(config);

            services.AddRapidCMSApiCore(_rootConfig);

            if (!_rootConfig.AllowAnonymousUsage)
            {
                services.AddSingleton<IUserResolver, UserResolver>();
            }

            _routeConvention = new CollectionControllerRouteConvention();

            return services;
        }

        public static IMvcBuilder AddRapidCMSControllers(this IServiceCollection services, Action<MvcOptions>? extraConfig = default)
        {
            if (_rootConfig == null || _routeConvention == null)
            {
                throw new InvalidOperationException("Call AddRapidCMSApi() before calling this method.");
            }

            var builder = services
                .AddControllers(config =>
                {
                    config.Conventions.Add(_routeConvention);

                    extraConfig?.Invoke(config);
                })
                .AddNewtonsoftJson(options =>
                {
                    // these settings are for reading json only
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
                });

            return builder;
        }

        private static ApiConfig GetRootConfig(Action<IApiConfig>? config = null)
        {
            var rootConfig = new ApiConfig();
            config?.Invoke(rootConfig);
            return rootConfig;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Dispatchers.Api;
using RapidCMS.Core.Factories;
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
    // TODO: merge with WebApi 
    public static class RapidCMSMiddlewareFunctions
    {
        private static ApiConfig? _rootConfig;

        public static IServiceCollection AddRapidCMSFunctions(this IServiceCollection services, Action<IApiConfig>? config = null)
        {
            _rootConfig = GetRootConfig(config);

            services.AddSingleton<IApiConfig>(_rootConfig);

            services.AddHttpContextAccessor();

            // TODO: implement properly
            services.AddTransient<IAuthorizationService, AuthorizationServiceShiv>();

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

            // TODO: functions to add?
            // - how to limit the repository aliases allowed like the api repository controllers

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

            // TODO: file uploaders to add?

            return services;
        }

        private static ApiConfig GetRootConfig(Action<IApiConfig>? config = null)
        {
            var rootConfig = new ApiConfig();
            config?.Invoke(rootConfig);
            return rootConfig;
        }
    }

    public class AuthorizationServiceShiv : IAuthorizationService
    {
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(AuthorizationResult.Success());
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
        {
            return Task.FromResult(AuthorizationResult.Success());
        }
    }
}

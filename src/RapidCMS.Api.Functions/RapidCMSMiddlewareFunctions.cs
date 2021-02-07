using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RapidCMS.Api.Core;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Models.Config.Api;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RapidCMSMiddlewareFunctions
    {
        private static ApiConfig? _rootConfig;

        public static IServiceCollection AddRapidCMSFunctions(this IServiceCollection services, Action<IApiConfig>? config = null)
        {

            _rootConfig = GetRootConfig(config);

            services.AddRapidCMSApiCore(_rootConfig);

            if (!_rootConfig.AllowAnonymousUsage)
            {
                // TODO: implement authentication state provider or user resolver
            }

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

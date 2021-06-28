using System;
using RapidCMS.Api.Core;
using RapidCMS.Api.Functions.Abstractions;
using RapidCMS.Api.Functions.Accessors;
using RapidCMS.Api.Functions.Resolvers;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
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

            services.AddSingleton<IFunctionContextAccessor, FunctionExecutionContextAccessor>();

            if (!_rootConfig.AllowAnonymousUsage)
            {
                services.AddSingleton<IUserResolver, UserResolver>();
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
}

using System;
using System.Threading;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Services.Auth;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class RapidCMSMiddleware
    {
        /// <summary>
        /// Use this method to configure RapidCMS to run on a Blazor WebAssembly App, fully client side.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddRapidCMSWebAssembly(this IServiceCollection services, Action<ICmsConfig>? config = null)
        {
            var rootConfig = GetRootConfig(config);

            services.AddTransient<IAuthService, WebAssemblyAuthService>();

            // TODO: implement proper API driver dataviewresolver
            services.AddTransient<IDataViewResolver, FormDataViewResolver>();

            // TODO
            // Semaphore for repositories
            services.AddSingleton(serviceProvider => new SemaphoreSlim(rootConfig.Advanced.SemaphoreCount, rootConfig.Advanced.SemaphoreCount));

            return services.AddRapidCMSCore(rootConfig);
        }
    }
}

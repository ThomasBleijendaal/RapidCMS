using System;
using System.Threading;
using Tewr.Blazor.FileReader;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Services.Auth;
using RapidCMS.Core.Enums;

namespace Microsoft.Extensions.DependencyInjection
{
    // TODO: disable authentication/ route in server mode
    public static partial class RapidCMSMiddleware
    {
        /// <summary>
        /// Use this method to configure RapidCMS to run on a Blazor Server App, fully server side.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddRapidCMSServer(this IServiceCollection services, Action<ICmsConfig>? config = null)
        {
            var rootConfig = GetRootConfig(CmsType.ServerSide, config);

            services.AddTransient<IAuthService, ServerSideAuthService>();

            services.AddTransient<IDataViewResolver, FormDataViewResolver>();

            services.AddFileReaderService();

            // Semaphore for repositories
            services.AddSingleton(serviceProvider => new SemaphoreSlim(rootConfig.Advanced.SemaphoreCount, rootConfig.Advanced.SemaphoreCount));

            return services.AddRapidCMSCore(rootConfig);
        }
    }
}

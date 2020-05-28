using System;
using System.Threading;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Services.Auth;
using RapidCMS.Repositories.ApiBridge;

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

        /// <summary>
        /// Adds a plain HttpClient for the given collection which is hosted at the given baseAddress.
        /// The ApiRepository uses this HttpClient to communicate to the server.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="baseAddress">Base address of the api, for example: https://example.com</param>
        /// <param name="collectionAlias"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddRapidCMSApiHttpClient<TEntity>(this IServiceCollection services, Uri baseUri, string collectionAlias)
            where TEntity : class, IEntity
        {
            return services.AddHttpClient<ApiRepository<TEntity>>(collectionAlias)
                .ConfigureHttpClient(x => x.BaseAddress = new Uri(baseUri, $"api/_rapidcms/{collectionAlias}/"));
        }

        /// <summary>
        /// Adds a plain HttpClient for the given collection which is hosted at the given baseAddress.
        /// The ApiMappedRepository uses this HttpClient to communicate to the server.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="baseAddress">Base address of the api, for example: https://example.com</param>
        /// <param name="collectionAlias"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddRapidCMSApiHttpClient<TEntity, TDatabaseEntity>(this IServiceCollection services, Uri baseUri, string collectionAlias)
            where TEntity : class, IEntity
            where TDatabaseEntity : class
        {
            return services.AddHttpClient<ApiMappedRepository<TEntity, TDatabaseEntity>>(collectionAlias)
                .ConfigureHttpClient(x => x.BaseAddress = new Uri(baseUri, $"api/_rapidcms/{collectionAlias}/"));
        }
    }
}

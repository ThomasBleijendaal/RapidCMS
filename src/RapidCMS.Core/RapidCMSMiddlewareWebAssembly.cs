using System;
using System.Threading;
using Blazor.FileReader;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Helpers;
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

            services.AddFileReaderService();

            return services.AddRapidCMSCore(rootConfig);
        }

        /// <summary>
        /// Adds the repository as scoped service and adds a plain HttpClient for the given repository.
        /// </summary>
        /// <typeparam name="TIRepository"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddRapidCMSApiRepository<TIRepository, TRepository>(this IServiceCollection services, Uri baseUri)
            where TIRepository : class
            where TRepository : class, TIRepository
        {
            var alias = AliasHelper.GetRepositoryAlias(typeof(TRepository));

            services.AddScoped<TIRepository, TRepository>();

            return services.AddHttpClient(alias)
                .ConfigureHttpClient(x => x.BaseAddress = new Uri(baseUri, $"api/_rapidcms/{alias}/"));
        }

        /// <summary>
        /// Adds a plain HttpClient for the given file upload handler which is hosted at the given baseAddress.
        /// The ApiFileHandler uses this HttpClient to communicate to the server.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="baseAddress">Base address of the api, for example: https://example.com</param>
        /// <param name="collectionAlias"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddRapidCMSFileUploadApiHttpClient<THandler>(this IServiceCollection services, Uri baseUri)
            where THandler : IFileUploadHandler
        {
            var alias = AliasHelper.GetFileUploaderAlias(typeof(THandler));

            return services.AddHttpClient<ApiFileUploadHandler<THandler>>(alias)
                .ConfigureHttpClient(x => x.BaseAddress = new Uri(baseUri, $"api/_rapidcms/{alias}/"));
        }
    }
}

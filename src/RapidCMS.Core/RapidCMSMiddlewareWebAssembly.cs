using System;
using System.Net.Http;
using System.Threading;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Services.Auth;
using Tewr.Blazor.FileReader;

namespace Microsoft.Extensions.DependencyInjection;

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
        services.AddTransient<IDataViewResolver, FormDataViewResolver>();

        services.AddSingleton(serviceProvider => new SemaphoreSlim(rootConfig.Advanced.SemaphoreCount, rootConfig.Advanced.SemaphoreCount));

        services.AddFileReaderService();

        return services.AddRapidCMSCore(rootConfig);
    }

    /// <summary>
    /// Adds the repository as scoped service and adds a plain HttpClient for the given repository.
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <param name="services"></param>
    /// <param name="baseUri"></param>
    /// <returns></returns>
    public static IHttpClientBuilder AddRapidCMSApiRepository<TRepository>(this IServiceCollection services, Uri baseUri)
        where TRepository : class
    {
        return services.AddRapidCMSApiRepository<TRepository, TRepository>(baseUri);
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

        var builder = services.AddHttpClient(alias)
            .ConfigureHttpClient(x => x.BaseAddress = new Uri(baseUri, $"_rapidcms/{alias}/"));

        return builder;
    }

    /// <summary>
    /// Adds the repository as scoped service and adds an HttpClient with TDelegatingHandler for the given repository.
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TDelegatingHandler"></typeparam>
    /// <param name="services"></param>
    /// <param name="baseUri"></param>
    /// <returns></returns>
    public static IHttpClientBuilder AddRapidCMSAuthenticatedApiRepository<TRepository, TDelegatingHandler>(this IServiceCollection services, Uri baseUri)
        where TRepository : class
        where TDelegatingHandler : DelegatingHandler
    {
        return services.AddRapidCMSAuthenticatedApiRepository<TRepository, TRepository, TDelegatingHandler>(baseUri);
    }

    /// <summary>
    /// Adds the repository as scoped service and adds an HttpClient with TDelegatingHandler for the given repository.
    /// </summary>
    /// <typeparam name="TIRepository"></typeparam>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TDelegatingHandler"></typeparam>
    /// <param name="services"></param>
    /// <param name="baseUri"></param>
    /// <returns></returns>
    public static IHttpClientBuilder AddRapidCMSAuthenticatedApiRepository<TIRepository, TRepository, TDelegatingHandler>(this IServiceCollection services, Uri baseUri)
        where TIRepository : class
        where TRepository : class, TIRepository
        where TDelegatingHandler : DelegatingHandler
    {
        var builder = services.AddRapidCMSApiRepository<TIRepository, TRepository>(baseUri);

        if (_tokenMessageHandlerBuilder != null && _tokenMessageHandlerBuilder is Func<IServiceProvider, TDelegatingHandler> messageHandlerBuilder)
        { 
            builder = builder.AddHttpMessageHandler(sp => messageHandlerBuilder.Invoke(sp));
        }
        else
        {
            builder = builder.AddHttpMessageHandler<TDelegatingHandler>();
        } 

        return builder;
    }

    private static Func<IServiceProvider, DelegatingHandler>? _tokenMessageHandlerBuilder = null;

    /// <summary>
    /// Adds the MessageHandler which adds AccessTokens to HttClient-requests to ApiRepository-calls.
    /// 
    /// NOTE: Set before calling AddRapidCMSApiRepository or AddRapidCMSFileUploadApiHttpClient
    /// </summary>
    /// <param name="services"></param>
    /// <param name="handlerBuilder"></param>
    /// <returns></returns>
    public static IServiceCollection AddRapidCMSApiTokenAuthorization<TDelegatingHandler>(this IServiceCollection services, Func<IServiceProvider, TDelegatingHandler> handlerBuilder)
        where TDelegatingHandler : DelegatingHandler
    {
        _tokenMessageHandlerBuilder = handlerBuilder;

        return services;
    }

    /// <summary>
    /// Adds a plain HttpClient for the given file upload handler which is hosted at the given baseAddress.
    /// The ApiFileHandler uses this HttpClient to communicate to the server.
    /// </summary>
    /// <typeparam name="TFileHandler"></typeparam>
    /// <param name="services"></param>
    /// <param name="baseUri">Base address of the api, for example: https://example.com</param>
    /// <returns></returns>
    public static IHttpClientBuilder AddRapidCMSFileUploadApiHttpClient<TFileHandler>(this IServiceCollection services, Uri baseUri)
        where TFileHandler : IFileUploadHandler
    {
        var alias = AliasHelper.GetFileUploaderAlias(typeof(TFileHandler));

        var builder = services.AddHttpClient<ApiFileUploadHandler<TFileHandler>>(alias)
            .ConfigureHttpClient(x => x.BaseAddress = new Uri(baseUri, $"_rapidcms/{alias}/"));

        return builder;
    }

    /// <summary>
    /// Adds a plain HttpClient for the given file upload handler which is hosted at the given baseAddress.
    /// The ApiFileHandler uses this HttpClient to communicate to the server.
    /// </summary>
    /// <typeparam name="TFileHandler"></typeparam>
    /// <typeparam name="TDelegatingHandler"></typeparam>
    /// <param name="services"></param>
    /// <param name="baseUri">Base address of the api, for example: https://example.com</param>
    /// <returns></returns>
    public static IHttpClientBuilder AddRapidCMSAuthenticatedFileUploadApiHttpClient<TFileHandler, TDelegatingHandler>(this IServiceCollection services, Uri baseUri)
        where TFileHandler : IFileUploadHandler
        where TDelegatingHandler : DelegatingHandler
    {
        var builder = services.AddRapidCMSFileUploadApiHttpClient<TFileHandler>(baseUri);

        if (_tokenMessageHandlerBuilder != null && _tokenMessageHandlerBuilder is Func<IServiceProvider, TDelegatingHandler> messageHandlerBuilder)
        {
            builder = builder.AddHttpMessageHandler(sp => messageHandlerBuilder.Invoke(sp));
        }
        else
        {
            builder = builder.AddHttpMessageHandler<TDelegatingHandler>();
        }

        return builder;
    }
}

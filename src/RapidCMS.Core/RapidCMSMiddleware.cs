using System;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Dispatchers;
using RapidCMS.Core.Factories.UIResolverFactory;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Resolvers.Data;
using RapidCMS.Core.Resolvers.Repositories;
using RapidCMS.Core.Services.Exceptions;
using RapidCMS.Core.Services.Messages;
using RapidCMS.Core.Services.Parent;
using RapidCMS.Core.Services.Persistence;
using RapidCMS.Core.Services.SidePane;
using RapidCMS.Core.Services.Tree;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RapidCMSMiddleware
    {
        public static IServiceCollection AddRapidCMS(this IServiceCollection services, Action<ICmsConfig>? config = null)
        {
            var rootConfig = new CmsConfig();
            config?.Invoke(rootConfig);

            var cmsSetup = new CmsSetup(rootConfig);

            services.AddSingleton<ICms>(cmsSetup);
            services.AddSingleton<IDashboard>(cmsSetup);
            services.AddSingleton<ILogin>(cmsSetup);

            if (cmsSetup.AllowAnonymousUsage)
            {
                services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
                services.AddSingleton<AuthenticationStateProvider, AnonymousAuthenticationStateProvider>();
            }

            services.AddTransient<IUIResolverFactory, UIResolverFactory>();

            services.AddSingleton<ICollectionResolver>(cmsSetup);
            services.AddTransient<IRepositoryResolver, RepositoryResolver>();
            services.AddTransient<IDataProviderResolver, DataProviderResolver>();

            services.AddTransient<IDispatcher<GetEntityRequestModel, EntityResponseModel>, GetEntityDispatcher>();

            services.AddSingleton<IExceptionService, ExceptionService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddTransient<IParentService, ParentService>();
            services.AddTransient<IPersistenceService, PersistenceService>();
            services.AddScoped<ISidePaneService, SidePaneService>();
            services.AddTransient<ITreeService, TreeService>();

            services.AddScoped<DefaultButtonActionHandler>();
            services.AddScoped(typeof(OpenPaneButtonActionHandler<>));

            services.AddScoped(typeof(EnumDataProvider<>), typeof(EnumDataProvider<>));

            // UI requirements
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();

            services.AddHttpClient();
            services.AddScoped<HttpClient>();

            // Scoped semaphore for repositories
            services.AddScoped(serviceProvider => new SemaphoreSlim(rootConfig.SemaphoreMaxCount, rootConfig.SemaphoreMaxCount));

            services.AddMemoryCache();

            return services;
        }

        public static IApplicationBuilder UseRapidCMS(this IApplicationBuilder app, bool isDevelopment = false)
        {
            app.ApplicationServices.GetService<CmsConfig>().IsDevelopment = isDevelopment;

            return app;
        }
    }
}

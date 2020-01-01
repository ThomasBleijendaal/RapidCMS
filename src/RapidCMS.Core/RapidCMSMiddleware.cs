using System;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Resolvers;
using RapidCMS.Core.Resolvers.Repositories;
using RapidCMS.Core.Services;
using RapidCMS.Core.Services.Parent;
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
            services.AddSingleton<ICollections>(cmsSetup);
            services.AddSingleton<IDashboard>(cmsSetup);
            services.AddSingleton<ILogin>(cmsSetup);

            if (cmsSetup.AllowAnonymousUsage)
            {
                services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
                services.AddSingleton<AuthenticationStateProvider, AnonymousAuthenticationStateProvider>();
            }

            ////  UI + Repository services
            services.AddTransient<IRepositoryResolver, RepositoryResolver>();
            //services.AddTransient<IDataProviderService, DataProviderService>();
            //services.AddScoped<IEditContextService, EditContextService>();
            //services.AddTransient<IEditorService, EditorService>();
            services.AddTransient<ITreeService, TreeService>();
            services.AddTransient<IParentService, ParentService>();

            //// Data exchange services
            services.AddScoped<ISidePaneService, SidePaneService>();
            //services.AddScoped<IMessageService, MessageService>();

            //// Button handlers
            //services.AddScoped<DefaultButtonActionHandler>();
            //services.AddScoped(typeof(OpenPaneButtonActionHandler<>));

            //// Debug helpers
            //services.AddScoped<IExceptionHelper, ExceptionHelper>();

            //// Stock data providers
            //services.AddScoped(typeof(EnumDataProvider<>), typeof(EnumDataProvider<>));

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

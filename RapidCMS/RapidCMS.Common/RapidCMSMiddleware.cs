using System;
using System.Net.Http;
using System.Threading;
using EventAggregator.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Authorization;
using RapidCMS.Common.Data;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RapidCMSMiddleware
    {
        public static IServiceCollection AddRapidCMS(this IServiceCollection services, Action<CmsConfig>? config = null)
        {
            var rootConfig = new CmsConfig();
            config?.Invoke(rootConfig);

            if (rootConfig.AllowAnonymousUsage)
            {
                services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
            }

            services.AddSingleton(rootConfig);

            services.AddScoped<Root>();

            services.AddTransient<IEditContextService, EditContextService>();
            services.AddTransient<IEditorService, EditorService>();
            services.AddTransient<ITreeService, TreeService>();

            services.AddScoped<IExceptionHelper, ExceptionHelper>();

            services.AddSingleton(typeof(EnumDataProvider<>), typeof(EnumDataProvider<>));

            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();

            services.AddHttpClient();
            services.AddScoped<HttpClient>();

            // scoped semaphore for repositories
            services.AddScoped(serviceProvider => new SemaphoreSlim(1, 1));

            services.AddSingleton<IEventAggregator, EventAggregator.Blazor.EventAggregator>();

            return services;
        }

        public static IApplicationBuilder UseRapidCMS(this IApplicationBuilder app, bool isDevelopment = false)
        {
            app.ApplicationServices.GetService<CmsConfig>().IsDevelopment = isDevelopment;

            return app;
        }
    }
}

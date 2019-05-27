using System;
using Microsoft.AspNetCore.Builder;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.Services;
using RapidCMS.Common.ValueMappers;

#nullable enable

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RapidCMSMiddleware
    {
        public static IServiceCollection AddRapidCMS(this IServiceCollection services, Action<CmsConfig>? config = null)
        {
            var rootConfig = new CmsConfig();
            config?.Invoke(rootConfig);

            services.AddSingleton(rootConfig);

            services.AddScoped<Root>();

            services.AddTransient<ICollectionService, CollectionService>();
            services.AddTransient<IUIService, UIService>();

            services.AddSingleton<DefaultValueMapper>();
            services.AddSingleton<LongValueMapper>();
            services.AddSingleton<BoolValueMapper>();
            services.AddSingleton(typeof(CollectionValueMapper<>), typeof(CollectionValueMapper<>));

            return services;
        }

        public static IApplicationBuilder UseRapidCMS(this IApplicationBuilder app)
        {
            ServiceLocator.CreateInstance(app.ApplicationServices);

            //var root = app.ApplicationServices.GetRequiredService<Root>();

            //configure.Invoke(root);

            return app;
        }
    }
}

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.Services;
using RapidCMS.Common.ValueMappers;

#nullable enable

namespace RapidCMS.Common.Extensions
{
    public static class RapidCMSMiddleware
    {
        public static IServiceCollection AddRapidCMS(this IServiceCollection services, Action<RootConfig>? config = null)
        {
            var rootConfig = new RootConfig();
            config?.Invoke(rootConfig);

            var root = new Root(
                rootConfig.CustomButtonRegistrations,
                rootConfig.CustomEditorRegistrations,
                rootConfig.CustomSectionRegistrations);

            services.AddSingleton(root);
            services.AddSingleton<ICollectionService, CollectionService>();
            services.AddSingleton<IUIService, UIService>();

            services.AddSingleton<DefaultValueMapper>();
            services.AddSingleton<LongValueMapper>();
            services.AddSingleton<BoolValueMapper>();
            services.AddSingleton(typeof(CollectionValueMapper<>), typeof(CollectionValueMapper<>));

            return services;
        }

        public static IApplicationBuilder UseRapidCMS(this IApplicationBuilder app, Action<Root> configure)
        {
            ServiceLocator.CreateInstance(app.ApplicationServices);

            var root = app.ApplicationServices.GetRequiredService<Root>();

            configure.Invoke(root);

            try
            {
                root.MaterializeRepositories(app.ApplicationServices);
            }
            catch
            {

            }

            return app;
        }
    }
}

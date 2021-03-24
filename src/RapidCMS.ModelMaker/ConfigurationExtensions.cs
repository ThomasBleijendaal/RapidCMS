using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;

namespace RapidCMS.ModelMaker
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddModelMaker(this IServiceCollection services)
        {
            // TODO: what should this life time be?
            services.AddTransient<IPlugin, ModelMakerPlugin>();
            services.AddTransient<ModelMakerRepository>();

            return services;
        }

        public static ICmsConfig AddModelMakerPlugin(this ICmsConfig cmsConfig)
        {
            cmsConfig.AddPlugin<ModelMakerPlugin>();

            return cmsConfig;
        }
    }
}

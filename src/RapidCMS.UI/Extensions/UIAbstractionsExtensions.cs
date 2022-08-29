using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.UI;

namespace RapidCMS.UI.Extensions;

public static class UIAbstractionsExtensions
{
    public static async Task<TConfig?> GetConfigAsync<TConfig>(this IWantConfiguration<TConfig> element)
        where TConfig : class
    {
        if (element.Configuration == null)
        {
            return null;
        }

        var config = await element.Configuration.Invoke(element.Entity, element.EntityState);
        if (config is not null and not TConfig)
        {
            throw new InvalidOperationException($"UI Element wants config of type '{typeof(TConfig).Name}' but got '{config.GetType().Name}'. Update .");
        }

        return config as TConfig;
    }

    public static async Task<TConfig> GetConfigAsync<TConfig>(this IRequireConfiguration<TConfig> element)
        where TConfig : class
    {
        if (element.Configuration == null)
        {
            throw new InvalidOperationException($"UI Element requires config of type '{typeof(TConfig).Name}' but no configuration was set.");
        }

        var config = await element.Configuration.Invoke(element.Entity, element.EntityState);
        if (config is not null and not TConfig)
        {
            throw new InvalidOperationException($"UI Element requires config of type '{typeof(TConfig).Name}' but got '{config.GetType().Name}'. Update .");
        }

        return config as TConfig ?? throw new InvalidOperationException($"UI Element requires config of type '{typeof(TConfig).Name}' but got null.");
    }
}

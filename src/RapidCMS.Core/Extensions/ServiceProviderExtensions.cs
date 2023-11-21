using System;

namespace RapidCMS.Core.Extensions;

internal static class ServiceProviderExtensions
{
    public static T GetService<T>(this IServiceProvider serviceProvider, Type type)
        where T : class
    {
        return (serviceProvider.GetService(type) ?? throw new InvalidOperationException($"Failed to resolve instance of type {type}"))
            as T ?? throw new InvalidOperationException($"Resolved instance of type {type}, but failed to cast it as {typeof(T)}");
    }

    public static T GetService<T>(this IServiceProvider serviceProvider)
        where T : class
    {
        return serviceProvider.GetService(typeof(T)) as T ?? throw new InvalidOperationException($"Failed to resolve instance of type {typeof(T)}");
    }
}

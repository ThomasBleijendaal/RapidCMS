using System;

namespace RapidCMS.Common.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider, Type type)
            where T : class
        {
            return serviceProvider.GetService(type) as T ?? throw new InvalidOperationException($"Failed to resolve instance of type {type} and cast it as {typeof(T)}");
        }

        public static T GetService<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T ?? throw new InvalidOperationException($"Failed to resolve instance of type {typeof(T)}");
        }
    }
}

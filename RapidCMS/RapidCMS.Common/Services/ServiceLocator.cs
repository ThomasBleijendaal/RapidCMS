using System;
using Microsoft.Extensions.DependencyInjection;

namespace RapidCMS.Common.Services
{
    [Obsolete]
    internal class ServiceLocator
    {
        private readonly IServiceProvider _serviceProvider;
        [Obsolete]
        public ServiceLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        [Obsolete]
        public T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
        [Obsolete]
        public T GetService<T>(Type t)
        {
            return (T)_serviceProvider.GetRequiredService(t);
        }
        [Obsolete]
        public static ServiceLocator Instance { get; private set; }
        [Obsolete]
        public static void CreateInstance(IServiceProvider serviceProvider)
        {
            Instance = new ServiceLocator(serviceProvider);
        }
    }
}

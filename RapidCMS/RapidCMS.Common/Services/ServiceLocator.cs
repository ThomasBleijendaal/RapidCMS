using System;
using Microsoft.Extensions.DependencyInjection;

namespace RapidCMS.Common.Services
{
    public class ServiceLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public T GetService<T>(Type t)
        {
            return (T)_serviceProvider.GetRequiredService(t);
        }

        public static ServiceLocator Instance { get; private set; }

        public static void CreateInstance(IServiceProvider serviceProvider)
        {
            Instance = new ServiceLocator(serviceProvider);
        }
    }
}

using System;
using System.Linq;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Core.Handlers;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Api.Core.Resolvers
{
    internal class FileHandlerResolver : IFileHandlerResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IApiConfig _config;

        public FileHandlerResolver(
            IServiceProvider serviceProvider,
            IApiConfig config)
        {
            _serviceProvider = serviceProvider;
            _config = config;
        }

        public IFileHandler GetFileHandler(string uploadHandlerAlias)
        {
            var handlerConfig = _config.FileUploadHandlers.FirstOrDefault(x => x.Alias == uploadHandlerAlias);
            if (handlerConfig == null)
            {
                throw new InvalidOperationException();
            }

            var handlerType = typeof(FileHandler<>).MakeGenericType(handlerConfig.HandlerType);

            return _serviceProvider.GetService<IFileHandler>(handlerType);
        }
    }
}

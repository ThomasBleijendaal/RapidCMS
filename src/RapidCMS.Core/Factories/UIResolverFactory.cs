using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Resolvers.UI;

namespace RapidCMS.Core.Factories
{
    internal class UIResolverFactory : IUIResolverFactory
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly IDataProviderResolver _dataProviderResolver;
        private readonly IButtonActionHandlerResolver _buttonActionHandlerResolver;
        private readonly IDataViewResolver _dataViewResolver;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UIResolverFactory(
            ISetupResolver<ICollectionSetup> collectionResolver,
            IDataProviderResolver dataProviderResolver,
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            IDataViewResolver dataViewResolver,
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor)
        {
            _collectionResolver = collectionResolver;
            _dataProviderResolver = dataProviderResolver;
            _buttonActionHandlerResolver = buttonActionHandlerResolver;
            _dataViewResolver = dataViewResolver;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<INodeUIResolver> GetNodeUIResolverAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _collectionResolver.ResolveSetup(collectionAlias);
            var node = usageType.HasFlag(UsageType.View)
                ? collection.NodeView ?? collection.NodeEditor
                : collection.NodeEditor ?? collection.NodeView;
            if (node == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            INodeUIResolver nodeUI = new NodeUIResolver(node, _dataProviderResolver, _buttonActionHandlerResolver,  _authService, _httpContextAccessor);

            return Task.FromResult(nodeUI);
        }

        public Task<IListUIResolver> GetListUIResolverAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _collectionResolver.ResolveSetup(collectionAlias);
            var list = usageType == UsageType.List || usageType.HasFlag(UsageType.Add)
                ? collection.ListView ?? collection.ListEditor
                : collection.ListEditor ?? collection.ListView;
            if (list == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            IListUIResolver listUI = new ListUIResolver(list, collection, _dataProviderResolver, _dataViewResolver, _buttonActionHandlerResolver, _authService, _httpContextAccessor);

            return Task.FromResult(listUI);
        }
    }
}

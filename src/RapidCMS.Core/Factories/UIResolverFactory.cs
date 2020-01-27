using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Resolvers.UI;

namespace RapidCMS.Core.Factories
{
    internal class UIResolverFactory : IUIResolverFactory
    {
        private readonly ICollectionResolver _collectionResolver;
        private readonly IDataProviderResolver _dataProviderResolver;
        private readonly IDataViewResolver _dataViewResolver;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UIResolverFactory(
            ICollectionResolver collectionResolver,
            IDataProviderResolver dataProviderResolver,
            IDataViewResolver dataViewResolver,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _collectionResolver = collectionResolver;
            _dataProviderResolver = dataProviderResolver;
            _dataViewResolver = dataViewResolver;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<INodeUIResolver> GetNodeUIResolverAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _collectionResolver.GetCollection(collectionAlias);
            var node = usageType.HasFlag(UsageType.View)
                ? collection.NodeView ?? collection.NodeEditor
                : collection.NodeEditor ?? collection.NodeView;
            if (node == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            INodeUIResolver nodeUI = new NodeUIResolver(node, _dataProviderResolver, _authorizationService, _httpContextAccessor);

            return Task.FromResult(nodeUI);
        }

        public Task<IListUIResolver> GetListUIResolverAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _collectionResolver.GetCollection(collectionAlias);
            var list = usageType == UsageType.List || usageType.HasFlag(UsageType.Add)
                ? collection.ListView ?? collection.ListEditor
                : collection.ListEditor ?? collection.ListView;
            if (list == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            IListUIResolver listUI = new ListUIResolver(list, collection, _dataProviderResolver, _dataViewResolver, _authorizationService, _httpContextAccessor);

            return Task.FromResult(listUI);
        }
    }
}

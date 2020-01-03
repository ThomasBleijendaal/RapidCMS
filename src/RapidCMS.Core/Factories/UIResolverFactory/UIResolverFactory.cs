using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Resolvers.UI;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Resolvers;
using RapidCMS.Core.Resolvers.UI;

namespace RapidCMS.Core.Factories.UIResolverFactory
{
    internal class UIResolverFactory : IUIResolverFactory
    {
        private readonly ICollections _setup;
        private readonly IDataProviderResolver _dataProviderResolver;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UIResolverFactory(
            ICollections setup,
            IDataProviderResolver dataProviderResolver,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _setup = setup;
            _dataProviderResolver = dataProviderResolver;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<INodeUIResolver> GetNodeUIResolverAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _setup.GetCollection(collectionAlias);
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
            var collection = _setup.GetCollection(collectionAlias);
            var list = usageType == UsageType.List || usageType.HasFlag(UsageType.Add)
                ? collection.ListView ?? collection.ListEditor
                : collection.ListEditor ?? collection.ListView;
            if (list == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            IListUIResolver listUI = new ListUIResolver(list, collection, _dataProviderResolver, _authorizationService, _httpContextAccessor);

            return Task.FromResult(listUI);
        }
    }
}

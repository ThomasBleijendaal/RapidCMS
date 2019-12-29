using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Providers;
using RapidCMS.Common.Resolvers.UI;

namespace RapidCMS.Common.Services.UI
{
    internal class EditorService : IEditorService
    {
        private readonly ICollectionProvider _collectionProvider;
        private readonly IDataProviderService _dataProviderService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditorService(
            ICollectionProvider collectionProvider,
            IDataProviderService dataProviderService,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _collectionProvider = collectionProvider;
            _dataProviderService = dataProviderService;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<INodeUIResolver> GetNodeUIResolverAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _collectionProvider.GetCollection(collectionAlias);
            var node = usageType.HasFlag(UsageType.View) 
                ? (collection.NodeView ?? collection.NodeEditor)
                : (collection.NodeEditor ?? collection.NodeView);
            if (node == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            INodeUIResolver nodeUI = new NodeUIResolver(node, _dataProviderService, _authorizationService, _httpContextAccessor);

            return Task.FromResult(nodeUI);
        }

        public Task<IListUIResolver> GetListUIResolverAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _collectionProvider.GetCollection(collectionAlias);
            var list = usageType == UsageType.List || usageType.HasFlag(UsageType.Add)
                ? (collection.ListView ?? collection.ListEditor)
                : (collection.ListEditor ?? collection.ListView);
            if (list == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            IListUIResolver listUI = new ListUIResolver(list, collection, _dataProviderService, _authorizationService, _httpContextAccessor);

            return Task.FromResult(listUI);
        }
    }
}

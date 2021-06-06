using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;
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
        private readonly IConventionBasedResolver<INodeSetup> _conventionBasedNodeSetupResolver;

        public UIResolverFactory(
            ISetupResolver<ICollectionSetup> collectionResolver,
            IDataProviderResolver dataProviderResolver,
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            IDataViewResolver dataViewResolver,
            IAuthService authService,
            IConventionBasedResolver<INodeSetup> conventionBasedNodeSetupResolver)
        {
            _collectionResolver = collectionResolver;
            _dataProviderResolver = dataProviderResolver;
            _buttonActionHandlerResolver = buttonActionHandlerResolver;
            _dataViewResolver = dataViewResolver;
            _authService = authService;
            _conventionBasedNodeSetupResolver = conventionBasedNodeSetupResolver;
        }

        public async Task<INodeUIResolver> GetNodeUIResolverAsync(UsageType usageType, string collectionAlias)
        {
            var collection = await _collectionResolver.ResolveSetupAsync(collectionAlias);
            var node = usageType.HasFlag(UsageType.View)
                ? collection.NodeView ?? collection.NodeEditor
                : collection.NodeEditor ?? collection.NodeView;
            if (node == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            INodeUIResolver nodeUI = new NodeUIResolver(node, _dataProviderResolver, _buttonActionHandlerResolver,  _authService);

            return nodeUI;
        }

        public async Task<IListUIResolver> GetListUIResolverAsync(UsageType usageType, string collectionAlias)
        {
            var collection = await _collectionResolver.ResolveSetupAsync(collectionAlias);
            var list = usageType == UsageType.List
                ? collection.ListView ?? collection.ListEditor
                : collection.ListEditor ?? collection.ListView;
            if (list == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            IListUIResolver listUI = new ListUIResolver(list, _dataProviderResolver, _dataViewResolver, _buttonActionHandlerResolver, _authService);

            return listUI;
        }

        public async Task<INodeUIResolver> GetConventionNodeUIResolverAsync(Type model)
        {
            var node = await _conventionBasedNodeSetupResolver.ResolveByConventionAsync(model, Features.CanEdit, new CollectionSetup(default, default, "Ad-Hoc", "adhoc", "adhoc"));

            INodeUIResolver nodeUI = new NodeUIResolver(node, _dataProviderResolver, _buttonActionHandlerResolver, _authService);

            return nodeUI;
        }
    }
}

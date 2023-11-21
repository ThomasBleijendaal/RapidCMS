using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Navigation;
using RapidCMS.Core.Resolvers.UI;

namespace RapidCMS.Core.Factories;

internal class UIResolverFactory : IUIResolverFactory
{
    private readonly ISetupResolver<CollectionSetup> _collectionResolver;
    private readonly IDataProviderResolver _dataProviderResolver;
    private readonly IButtonActionHandlerResolver _buttonActionHandlerResolver;
    private readonly IDataViewResolver _dataViewResolver;
    private readonly IAuthService _authService;
    private readonly IConventionBasedResolver<NodeSetup> _conventionBasedNodeSetupResolver;
    private readonly INavigationStateProvider _navigationStateProvider;

    public UIResolverFactory(
        ISetupResolver<CollectionSetup> collectionResolver,
        IDataProviderResolver dataProviderResolver,
        IButtonActionHandlerResolver buttonActionHandlerResolver,
        IDataViewResolver dataViewResolver,
        IAuthService authService,
        IConventionBasedResolver<NodeSetup> conventionBasedNodeSetupResolver,
        INavigationStateProvider navigationStateProvider)
    {
        _collectionResolver = collectionResolver;
        _dataProviderResolver = dataProviderResolver;
        _buttonActionHandlerResolver = buttonActionHandlerResolver;
        _dataViewResolver = dataViewResolver;
        _authService = authService;
        _conventionBasedNodeSetupResolver = conventionBasedNodeSetupResolver;
        _navigationStateProvider = navigationStateProvider;
    }

    public async Task<INodeUIResolver> GetNodeUIResolverAsync(NavigationState navigationState)
    {
        var collection = await _collectionResolver.ResolveSetupAsync(navigationState.CollectionAlias);
        var node = navigationState.UsageType.HasFlag(UsageType.View)
            ? collection.NodeView ?? collection.NodeEditor
            : collection.NodeEditor ?? collection.NodeView;
        if (node == null)
        {
            throw new InvalidOperationException($"Failed to get UI configuration from collection {navigationState.CollectionAlias} for action {navigationState.UsageType}");
        }

        INodeUIResolver nodeUI = new NodeUIResolver(node, _dataProviderResolver, _buttonActionHandlerResolver, _navigationStateProvider, _authService);

        return nodeUI;
    }

    public async Task<IListUIResolver> GetListUIResolverAsync(NavigationState navigationState)
    {
        var collection = await _collectionResolver.ResolveSetupAsync(navigationState.CollectionAlias);
        var list = navigationState.UsageType == UsageType.List
            ? collection.ListView ?? collection.ListEditor
            : collection.ListEditor ?? collection.ListView;
        if (list == null)
        {
            throw new InvalidOperationException($"Failed to get UI configuration from collection {navigationState.CollectionAlias} for action {navigationState.UsageType}");
        }

        IListUIResolver listUI = new ListUIResolver(list, _dataProviderResolver, _dataViewResolver, _buttonActionHandlerResolver, _navigationStateProvider, _authService);

        return listUI;
    }

    public async Task<INodeUIResolver> GetConventionNodeUIResolverAsync(Type model)
    {
        var node = await _conventionBasedNodeSetupResolver.ResolveByConventionAsync(model, Features.CanEdit, new CollectionSetup(default, default, "Ad-Hoc", "adhoc", "adhoc"));

        INodeUIResolver nodeUI = new NodeUIResolver(node, _dataProviderResolver, _buttonActionHandlerResolver, _navigationStateProvider, _authService);

        return nodeUI;
    }
}

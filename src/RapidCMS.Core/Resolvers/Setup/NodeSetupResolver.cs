using System;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup;

internal class NodeSetupResolver : ISetupResolver<NodeSetup, NodeConfig>
{
    private readonly ISetupResolver<PaneSetup, PaneConfig> _paneSetupResolver;
    private readonly ISetupResolver<ButtonSetup, ButtonConfig> _buttonSetupResolver;
    private readonly IConventionBasedResolver<NodeConfig> _conventionNodeConfigResolver;

    public NodeSetupResolver(
        ISetupResolver<PaneSetup, PaneConfig> paneSetupResolver,
        ISetupResolver<ButtonSetup, ButtonConfig> buttonSetupResolver,
        IConventionBasedResolver<NodeConfig> conventionNodeConfigResolver)
    {
        _paneSetupResolver = paneSetupResolver;
        _buttonSetupResolver = buttonSetupResolver;
        _conventionNodeConfigResolver = conventionNodeConfigResolver;
    }

    public async Task<IResolvedSetup<NodeSetup>> ResolveSetupAsync(NodeConfig config, CollectionSetup? collection = default)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (config is IIsConventionBased isConventionBasedConfig)
        {
            config = await _conventionNodeConfigResolver.ResolveByConventionAsync(config.BaseType, isConventionBasedConfig.GetFeatures(), collection);
        }

        var cacheable = true;

        var panes = (await _paneSetupResolver.ResolveSetupAsync(config.Panes, collection)).CheckIfCachable(ref cacheable).ToList();
        var buttons = (await _buttonSetupResolver.ResolveSetupAsync(config.Buttons, collection)).CheckIfCachable(ref cacheable).ToList();

        return new ResolvedSetup<NodeSetup>(new NodeSetup(
            config.BaseType,
            panes,
            buttons),
            cacheable);
    }
}

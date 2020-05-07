using System;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class NodeSetupResolver : ISetupResolver<NodeSetup, NodeConfig>
    {
        private readonly ISetupResolver<PaneSetup, PaneConfig> _paneSetupResolver;
        private readonly ISetupResolver<IButtonSetup, ButtonConfig> _buttonSetupResolver;

        public NodeSetupResolver(
            ISetupResolver<PaneSetup, PaneConfig> paneSetupResolver,
            ISetupResolver<IButtonSetup, ButtonConfig> buttonSetupResolver)
        {
            _paneSetupResolver = paneSetupResolver;
            _buttonSetupResolver = buttonSetupResolver;
        }

        public NodeSetup ResolveSetup(NodeConfig config, ICollectionSetup? collection = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (config is IIsConventionBased isConventionBasedConfig)
            {

            }

            var panes = _paneSetupResolver.ResolveSetup(config.Panes, collection).ToList();
            var buttons = _buttonSetupResolver.ResolveSetup(config.Buttons, collection).ToList();

            return new NodeSetup(
                config.BaseType,
                panes,
                buttons);
        }
    }
}

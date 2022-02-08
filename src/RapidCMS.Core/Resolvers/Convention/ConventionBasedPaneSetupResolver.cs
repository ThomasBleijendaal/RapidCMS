using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Convention
{
    internal class ConventionBasedNodeSetupResolver : IConventionBasedResolver<NodeSetup>
    {
        private readonly IConventionBasedResolver<NodeConfig> _nodeResolver;
        private readonly ISetupResolver<NodeSetup, NodeConfig> _nodeSetupResolver;

        public ConventionBasedNodeSetupResolver(
            IConventionBasedResolver<NodeConfig> nodeResolver,
            ISetupResolver<NodeSetup, NodeConfig> nodeSetupResolver)
        {
            _nodeResolver = nodeResolver;
            _nodeSetupResolver = nodeSetupResolver;
        }

        public async Task<NodeSetup> ResolveByConventionAsync(Type subject, Features features, CollectionSetup? collection)
        {
            var node = await _nodeResolver.ResolveByConventionAsync(subject, features, collection);
            var pane = await _nodeSetupResolver.ResolveSetupAsync(node, collection);

            return pane.Setup;
        }
    }
}

using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Resolvers.Convention
{
    internal class ConventionBasedNodeSetupResolver : IConventionBasedResolver<INodeSetup>
    {
        private readonly IConventionBasedResolver<NodeConfig> _nodeResolver;
        private readonly ISetupResolver<INodeSetup, NodeConfig> _nodeSetupResolver;

        public ConventionBasedNodeSetupResolver(
            IConventionBasedResolver<NodeConfig> nodeResolver,
            ISetupResolver<INodeSetup, NodeConfig> nodeSetupResolver)
        {
            _nodeResolver = nodeResolver;
            _nodeSetupResolver = nodeSetupResolver;
        }

        public async Task<INodeSetup> ResolveByConventionAsync(Type subject, Features features, ICollectionSetup? collection)
        {
            var node = await _nodeResolver.ResolveByConventionAsync(subject, features, collection);
            var pane = await _nodeSetupResolver.ResolveSetupAsync(node, collection);

            return pane.Setup;
        }
    }
}

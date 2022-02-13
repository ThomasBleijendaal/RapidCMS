using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TreeElementsSetupResolver : ISetupResolver<IEnumerable<TreeElementSetup>>
    {
        private readonly ICmsConfig _cmsConfig;
        private readonly ISetupResolver<IEnumerable<TreeElementSetup>, IEnumerable<ITreeElementConfig>> _treeElementResolver;
        private readonly ISetupResolver<IEnumerable<TreeElementSetup>, IPlugin> _pluginTreeElementResolver;
        private readonly IEnumerable<IPlugin> _plugins;

        public TreeElementsSetupResolver(ICmsConfig cmsConfig,
            ISetupResolver<IEnumerable<TreeElementSetup>, IEnumerable<ITreeElementConfig>> treeElementResolver,
            ISetupResolver<IEnumerable<TreeElementSetup>, IPlugin> pluginTreeElementResolver,
            IEnumerable<IPlugin> plugins)
        {
            _cmsConfig = cmsConfig;
            _treeElementResolver = treeElementResolver;
            _pluginTreeElementResolver = pluginTreeElementResolver;
            _plugins = plugins;
        }

        async Task<IEnumerable<TreeElementSetup>> ISetupResolver<IEnumerable<TreeElementSetup>>.ResolveSetupAsync()
        {
            var results = new List<TreeElementSetup>();

            foreach (var plugin in _plugins)
            {
                var treeElements = await _pluginTreeElementResolver.ResolveSetupAsync(plugin);

                results.AddRange(treeElements.Setup);
            }

            results.AddRange((await _treeElementResolver.ResolveSetupAsync(_cmsConfig.CollectionsAndPages?.Skip(1) ?? Enumerable.Empty<ITreeElementConfig>())).Setup);

            return results;
        }

        Task<IEnumerable<TreeElementSetup>> ISetupResolver<IEnumerable<TreeElementSetup>>.ResolveSetupAsync(string alias)
        {
            throw new InvalidOperationException("Cannot resolve root collections with alias.");
        }
    }
}

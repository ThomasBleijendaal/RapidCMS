using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TreeElementsSetupResolver : ISetupResolver<IEnumerable<ITreeElementSetup>>
    {
        private readonly ICmsConfig _cmsConfig;
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> _treeElementResolver;
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>, IPlugin> _pluginTreeElementResolver;
        private readonly IEnumerable<IPlugin> _plugins;

        public TreeElementsSetupResolver(ICmsConfig cmsConfig,
            ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> treeElementResolver,
            ISetupResolver<IEnumerable<ITreeElementSetup>, IPlugin> pluginTreeElementResolver,
            IEnumerable<IPlugin> plugins)
        {
            _cmsConfig = cmsConfig;
            _treeElementResolver = treeElementResolver;
            _pluginTreeElementResolver = pluginTreeElementResolver;
            _plugins = plugins;
        }

        async Task<IEnumerable<ITreeElementSetup>> ISetupResolver<IEnumerable<ITreeElementSetup>>.ResolveSetupAsync()
        {
            var results = new List<ITreeElementSetup>();

            foreach (var plugin in _plugins)
            {
                var treeElements = await _pluginTreeElementResolver.ResolveSetupAsync(plugin);

                results.AddRange(treeElements.Setup);
            }

            results.AddRange((await _treeElementResolver.ResolveSetupAsync(_cmsConfig.CollectionsAndPages?.Skip(1) ?? Enumerable.Empty<ITreeElementConfig>())).Setup);

            return results;
        }

        Task<IEnumerable<ITreeElementSetup>> ISetupResolver<IEnumerable<ITreeElementSetup>>.ResolveSetupAsync(string alias)
        {
            throw new InvalidOperationException("Cannot resolve root collections with alias.");
        }
    }
}

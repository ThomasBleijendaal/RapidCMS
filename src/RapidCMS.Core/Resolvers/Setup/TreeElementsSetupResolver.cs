using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Setup;

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

        IEnumerable<ITreeElementSetup> ISetupResolver<IEnumerable<ITreeElementSetup>>.ResolveSetup()
        {
            foreach (var plugin in _plugins)
            {
                var treeElements = _pluginTreeElementResolver.ResolveSetup(plugin);

                foreach (var element in treeElements.Setup)
                {
                    yield return element;
                }
            }

            foreach (var element in _treeElementResolver.ResolveSetup(_cmsConfig.CollectionsAndPages?.Skip(1) ?? Enumerable.Empty<ITreeElementConfig>()).Setup)
            {
                yield return element;
            }
        }

        IEnumerable<ITreeElementSetup> ISetupResolver<IEnumerable<ITreeElementSetup>>.ResolveSetup(string alias)
        {
            throw new InvalidOperationException("Cannot resolve root collections with alias.");
        }
    }

    internal class PluginTreeElementsSetupResolver : ISetupResolver<IEnumerable<ITreeElementSetup>, IPlugin>
    {
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> _treeElementResolver;

        public PluginTreeElementsSetupResolver(ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> treeElementResolver)
        {
            _treeElementResolver = treeElementResolver;
        }

        public IResolvedSetup<IEnumerable<ITreeElementSetup>> ResolveSetup(IPlugin config, ICollectionSetup? collection = null)
        {
            return new ResolvedSetup<IEnumerable<ITreeElementSetup>>(config.GetTreeElements(), false);
            
        }
    }
}

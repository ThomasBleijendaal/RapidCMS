using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class PluginTreeElementsSetupResolver : ISetupResolver<IEnumerable<ITreeElementSetup>, IPlugin>
    {
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> _treeElementResolver;

        public PluginTreeElementsSetupResolver(ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> treeElementResolver)
        {
            _treeElementResolver = treeElementResolver;
        }

        public async Task<IResolvedSetup<IEnumerable<ITreeElementSetup>>> ResolveSetupAsync(IPlugin config, ICollectionSetup? collection = null)
        {
            return new ResolvedSetup<IEnumerable<ITreeElementSetup>>(await config.GetTreeElementsAsync(), false);

        }
    }
}

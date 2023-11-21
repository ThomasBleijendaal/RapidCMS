using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup;

internal class PluginTreeElementsSetupResolver : ISetupResolver<IEnumerable<TreeElementSetup>, IPlugin>
{
    public async Task<IResolvedSetup<IEnumerable<TreeElementSetup>>> ResolveSetupAsync(IPlugin config, CollectionSetup? collection = null)
    {
        return new ResolvedSetup<IEnumerable<TreeElementSetup>>(await config.GetTreeElementsAsync(), false);

    }
}

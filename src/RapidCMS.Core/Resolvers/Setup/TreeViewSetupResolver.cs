using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TreeViewSetupResolver : ISetupResolver<ITreeViewSetup, TreeViewConfig>
    {
        public Task<IResolvedSetup<ITreeViewSetup>> ResolveSetupAsync(TreeViewConfig config, CollectionSetup? collection = default)
        {
            return Task.FromResult<IResolvedSetup<ITreeViewSetup>>(new ResolvedSetup<ITreeViewSetup>(new TreeViewSetup(
                config.EntityVisibilty,
                config.RootVisibility,
                config.DefaultOpenEntities,
                config.DefaultOpenCollections,
                config.Name),
                true));
        }
    }
}

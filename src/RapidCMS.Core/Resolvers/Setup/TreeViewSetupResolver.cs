using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TreeViewSetupResolver : ISetupResolver<TreeViewSetup, TreeViewConfig>
    {
        public Task<IResolvedSetup<TreeViewSetup>> ResolveSetupAsync(TreeViewConfig config, CollectionSetup? collection = default)
        {
            return Task.FromResult<IResolvedSetup<TreeViewSetup>>(new ResolvedSetup<TreeViewSetup>(new TreeViewSetup(
                config.EntityVisibilty,
                config.RootVisibility,
                config.DefaultOpenEntities,
                config.DefaultOpenCollections,
                config.Name),
                true));
        }
    }
}

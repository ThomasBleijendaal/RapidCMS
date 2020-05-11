using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TreeViewSetupResolver : ISetupResolver<TreeViewSetup, TreeViewConfig>
    {
        public IResolvedSetup<TreeViewSetup> ResolveSetup(TreeViewConfig config, ICollectionSetup? collection = default)
        {
            return new ResolvedSetup<TreeViewSetup>(new TreeViewSetup(
                config.EntityVisibilty,
                config.RootVisibility,
                config.DefaultOpenEntities,
                config.DefaultOpenCollections,
                config.Name),
                true);
        }
    }
}

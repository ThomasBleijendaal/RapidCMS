using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TreeViewSetupResolver : ISetupResolver<TreeViewSetup, TreeViewConfig>
    {
        public TreeViewSetup ResolveSetup(TreeViewConfig config, ICollectionSetup collection)
        {
            return new TreeViewSetup(
                config.EntityVisibilty,
                config.RootVisibility,
                config.DefaultOpenEntities,
                config.DefaultOpenCollections,
                config.Name);
        }
    }
}

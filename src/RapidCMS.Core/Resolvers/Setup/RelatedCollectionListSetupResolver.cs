using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class RelatedCollectionListSetupResolver : ISetupResolver<RelatedCollectionListSetup, CollectionListConfig>
    {
        public RelatedCollectionListSetup ResolveSetup(CollectionListConfig config, ICollectionSetup collection)
        {
            return new RelatedCollectionListSetup(config.Index, config.CollectionAlias);
        }
    }
}

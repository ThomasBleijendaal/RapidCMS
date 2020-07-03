using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class RelatedCollectionListSetupResolver : ISetupResolver<RelatedCollectionListSetup, CollectionListConfig>
    {
        public IResolvedSetup<RelatedCollectionListSetup> ResolveSetup(CollectionListConfig config, ICollectionSetup? collection = default)
        {
            return new ResolvedSetup<RelatedCollectionListSetup>(new RelatedCollectionListSetup(config.Index, config.CollectionAlias)
            {
                SupportsUsageType = (config?.ListEditor != null ? UsageType.Edit : 0) | (config?.ListView != null ? UsageType.View : 0)
            }, true);
        }
    }
}

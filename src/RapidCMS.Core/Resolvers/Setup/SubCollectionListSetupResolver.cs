using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class SubCollectionListSetupResolver : ISetupResolver<SubCollectionListSetup, CollectionListConfig>
    {
        public IResolvedSetup<SubCollectionListSetup> ResolveSetup(CollectionListConfig config, ICollectionSetup? collection = default)
        {
            return new ResolvedSetup<SubCollectionListSetup>(new SubCollectionListSetup(config.Index, config.CollectionAlias)
            {
                SupportsUsageType = (collection?.ListEditor != null ? UsageType.Edit : 0) | (collection?.ListView != null ? UsageType.View : 0)
            }, true);
        }
    }
}

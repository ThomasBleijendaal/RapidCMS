using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class SubCollectionListSetupResolver : ISetupResolver<SubCollectionListSetup, CollectionListConfig>
    {
        public SubCollectionListSetup ResolveSetup(CollectionListConfig config, ICollectionSetup? collection = default)
        {
            return new SubCollectionListSetup(config.Index, config.CollectionAlias);
        }
    }
}

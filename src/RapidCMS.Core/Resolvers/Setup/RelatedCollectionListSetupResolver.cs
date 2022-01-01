using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class RelatedCollectionListSetupResolver : CollectionListSetupResolverBase, ISetupResolver<IRelatedCollectionListSetup, CollectionListConfig>
    {
        public RelatedCollectionListSetupResolver(Lazy<ISetupResolver<ICollectionSetup>> setupResolver) : base(setupResolver)
        {
        }

        public async Task<IResolvedSetup<IRelatedCollectionListSetup>> ResolveSetupAsync(CollectionListConfig config, ICollectionSetup? collection = default)
        {
            var usageType = await GetUsageTypeAsync(config, collection);

            return new ResolvedSetup<IRelatedCollectionListSetup>(
                new RelatedCollectionListSetup(config.Index, config.CollectionAlias)
                {
                    SupportsUsageType = usageType
                }, true);
        }
    }
}

using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class RelatedCollectionListSetupResolver : CollectionListSetupResolverBase, ISetupResolver<RelatedCollectionListSetup, CollectionListConfig>
    {
        public RelatedCollectionListSetupResolver(Lazy<ISetupResolver<CollectionSetup>> setupResolver) : base(setupResolver)
        {
        }

        public async Task<IResolvedSetup<RelatedCollectionListSetup>> ResolveSetupAsync(CollectionListConfig config, CollectionSetup? collection = default)
        {
            var usageType = await GetUsageTypeAsync(config, collection);

            return new ResolvedSetup<RelatedCollectionListSetup>(
                new RelatedCollectionListSetup(config.Index, config.CollectionAlias)
                {
                    SupportsUsageType = usageType
                }, true);
        }
    }
}

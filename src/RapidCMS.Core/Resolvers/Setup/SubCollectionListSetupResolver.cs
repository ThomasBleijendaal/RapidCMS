using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class SubCollectionListSetupResolver : CollectionListSetupResolverBase, ISetupResolver<ISubCollectionListSetup, CollectionListConfig>
    {
        public SubCollectionListSetupResolver(Lazy<ISetupResolver<ICollectionSetup>> setupResolver) : base(setupResolver)
        {
        }

        public async Task<IResolvedSetup<ISubCollectionListSetup>> ResolveSetupAsync(CollectionListConfig config, ICollectionSetup? collection = default)
        {
            var usageType = await GetUsageTypeAsync(config, collection);

            return new ResolvedSetup<ISubCollectionListSetup>(
                new SubCollectionListSetup(config!.Index, config.CollectionAlias)
                {
                    SupportsUsageType = usageType
                }, true);
        }
    }
}

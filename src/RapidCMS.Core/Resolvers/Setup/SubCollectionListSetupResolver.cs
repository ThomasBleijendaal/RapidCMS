using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup;

internal class SubCollectionListSetupResolver : CollectionListSetupResolverBase, ISetupResolver<SubCollectionListSetup, CollectionListConfig>
{
    public SubCollectionListSetupResolver(Lazy<ISetupResolver<CollectionSetup>> setupResolver) : base(setupResolver)
    {
    }

    public async Task<IResolvedSetup<SubCollectionListSetup>> ResolveSetupAsync(CollectionListConfig config, CollectionSetup? collection = default)
    {
        var usageType = await GetUsageTypeAsync(config, collection);

        return new ResolvedSetup<SubCollectionListSetup>(
            new SubCollectionListSetup(config!.Index, config.CollectionAlias)
            {
                SupportsUsageType = usageType
            }, true);
    }
}

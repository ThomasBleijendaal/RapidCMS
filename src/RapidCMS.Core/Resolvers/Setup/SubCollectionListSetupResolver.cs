using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class SubCollectionListSetupResolver : ISetupResolver<ISubCollectionListSetup, CollectionListConfig>
    {
        private readonly Lazy<ISetupResolver<ICollectionSetup>> _setupResolver;

        public SubCollectionListSetupResolver(Lazy<ISetupResolver<ICollectionSetup>> setupResolver)
        {
            _setupResolver = setupResolver;
        }

        public async Task<IResolvedSetup<ISubCollectionListSetup>> ResolveSetupAsync(CollectionListConfig config, ICollectionSetup? collection = default)
        {
            UsageType usageType;
            
            if (config is ReferencedCollectionListConfig referencedCollectionList)
            {
                if (referencedCollectionList.CollectionAlias == collection?.Alias)
                {
                    usageType =
                        (collection.ListEditor != null ? UsageType.Edit : 0) |
                        (collection.ListView != null ? UsageType.View : 0);
                }
                else
                {
                    // TODO: this can trigger infinite loops / stack overflows..
                    // TODO: check collection subcollections before using the resolver

                    var resolver = _setupResolver.Value;

                    var referencedCollection = await resolver.ResolveSetupAsync(referencedCollectionList.CollectionAlias);

                    usageType =
                        (referencedCollection?.ListEditor != null ? UsageType.Edit : 0) |
                        (referencedCollection?.ListView != null ? UsageType.View : 0);
                }
            }
            else
            {
                usageType =
                    (config?.ListEditor != null ? UsageType.Edit : 0) |
                    (config?.ListView != null ? UsageType.View : 0);
            }

            return new ResolvedSetup<ISubCollectionListSetup>(
                new SubCollectionListSetup(config!.Index, config.CollectionAlias)
                {
                    SupportsUsageType = usageType
                }, true);
        }
    }
}

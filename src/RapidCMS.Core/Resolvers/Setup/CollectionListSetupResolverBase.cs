using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class CollectionListSetupResolverBase
    {
        protected readonly Lazy<ISetupResolver<ICollectionSetup>> _setupResolver;

        public CollectionListSetupResolverBase(Lazy<ISetupResolver<ICollectionSetup>> setupResolver)
        {
            _setupResolver = setupResolver;
        }

        protected async Task<UsageType> GetUsageTypeAsync(CollectionListConfig config, ICollectionSetup? collection)
        {
            if (config is ReferencedCollectionListConfig referencedCollectionList)
            {
                if (referencedCollectionList.CollectionAlias == collection?.Alias)
                {
                    return (collection.ListEditor != null ? UsageType.Edit : 0) |
                        (collection.ListView != null ? UsageType.View : 0);
                }
                else
                {
                    // TODO: this can trigger infinite loops / stack overflows..

                    var resolver = _setupResolver.Value;

                    var referencedCollection = await resolver.ResolveSetupAsync(referencedCollectionList.CollectionAlias);

                    return (referencedCollection?.ListEditor != null ? UsageType.Edit : 0) |
                        (referencedCollection?.ListView != null ? UsageType.View : 0);
                }
            }
            else
            {
                return (config?.ListEditor != null ? UsageType.Edit : 0) |
                    (config?.ListView != null ? UsageType.View : 0);
            }
        }
    }
}

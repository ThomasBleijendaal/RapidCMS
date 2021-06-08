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
        public Task<IResolvedSetup<ISubCollectionListSetup>> ResolveSetupAsync(CollectionListConfig config, ICollectionSetup? collection = default)
        {
            return Task.FromResult< IResolvedSetup<ISubCollectionListSetup>>(
                new ResolvedSetup<ISubCollectionListSetup>(
                    new SubCollectionListSetup(config.Index, config.CollectionAlias)
                    {
                        SupportsUsageType = (config?.ListEditor != null ? UsageType.Edit : 0) | (config?.ListView != null ? UsageType.View : 0)
                    }, true));
        }
    }
}

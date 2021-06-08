using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TreeElementSetupResolver : ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>>
    {
        public Task<IResolvedSetup<IEnumerable<ITreeElementSetup>>> ResolveSetupAsync(IEnumerable<ITreeElementConfig> config, ICollectionSetup? collection = default)
        {
            return Task.FromResult< IResolvedSetup<IEnumerable<ITreeElementSetup>>>(
                new ResolvedSetup<IEnumerable<ITreeElementSetup>>(
                    config.Select(corp =>
                    {
                        var type = corp switch
                        {
                            IPageConfig page => PageType.Page,
                            _ => PageType.Collection
                        };

                        return new TreeElementSetup(corp.Alias, corp.Name, type)
                        {
                            RootVisibility = (corp as CollectionConfig)?.TreeView?.RootVisibility ?? default
                        };

                    }) ?? Enumerable.Empty<ITreeElementSetup>(),
                    true));
        }
    }
}

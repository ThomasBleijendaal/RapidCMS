using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TreeElementSetupResolver : ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>>
    {
        public IEnumerable<ITreeElementSetup> ResolveSetup(IEnumerable<ITreeElementConfig> config)
        {
            return config.Select(corp =>
            {
                var type = corp switch
                {
                    IPageConfig page => PageType.Page,
                    _ => PageType.Collection
                };

                return new TreeElementSetup(corp.Alias, type)
                {
                    RootVisibility = (corp as CollectionConfig)?.TreeView?.RootVisibility ?? default,
                    DefaultOpenCollections = (corp as CollectionConfig)?.TreeView?.DefaultOpenCollections ?? default
                };

            }) ?? Enumerable.Empty<ITreeElementSetup>();
        }
    }
}

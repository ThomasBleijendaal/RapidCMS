using System;
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
    internal class TreeElementSetupResolver : ISetupResolver<IEnumerable<ITreeElementSetup>>
    {
        private readonly ICmsConfig _cmsConfig;

        public TreeElementSetupResolver(ICmsConfig cmsConfig)
        {
            _cmsConfig = cmsConfig;
        }

        IEnumerable<ITreeElementSetup> ISetupResolver<IEnumerable<ITreeElementSetup>>.ResolveSetup()
        {
            return _cmsConfig.CollectionsAndPages?.Skip(1).Select(corp =>
            {
                var type = corp switch
                {
                    IPageConfig page => PageType.Page,
                    _ => PageType.Collection
                };

                return new TreeElementSetup(corp.Alias, type);
            }) ?? Enumerable.Empty<ITreeElementSetup>();
        }

        IEnumerable<ITreeElementSetup> ISetupResolver<IEnumerable<ITreeElementSetup>>.ResolveSetup(string alias)
        {
            throw new InvalidOperationException("Cannot resolve root collections with alias.");
        }
    }
}

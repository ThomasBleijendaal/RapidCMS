using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class TreeElementsSetupResolver : ISetupResolver<IEnumerable<ITreeElementSetup>>
    {
        private readonly ICmsConfig _cmsConfig;
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> _treeElementResolver;

        public TreeElementsSetupResolver(ICmsConfig cmsConfig,
            ISetupResolver<IEnumerable<ITreeElementSetup>, IEnumerable<ITreeElementConfig>> treeElementResolver)
        {
            _cmsConfig = cmsConfig;
            _treeElementResolver = treeElementResolver;
        }

        IEnumerable<ITreeElementSetup> ISetupResolver<IEnumerable<ITreeElementSetup>>.ResolveSetup()
        {
            return _treeElementResolver.ResolveSetup(_cmsConfig.CollectionsAndPages?.Skip(1) ?? Enumerable.Empty<ITreeElementConfig>());
        }

        IEnumerable<ITreeElementSetup> ISetupResolver<IEnumerable<ITreeElementSetup>>.ResolveSetup(string alias)
        {
            throw new InvalidOperationException("Cannot resolve root collections with alias.");
        }
    }
}

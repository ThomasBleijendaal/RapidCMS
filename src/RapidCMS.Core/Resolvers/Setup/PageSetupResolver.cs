using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class PageSetupResolver : ISetupResolver<IPageSetup>
    {
        private readonly ICmsConfig _cmsConfig;
        private readonly ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig> _typeRegistrationSetupResolver;
        private readonly Dictionary<string, IPageSetup> _cache = new Dictionary<string, IPageSetup>();

        public PageSetupResolver(
            ICmsConfig cmsConfig,
            ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig> typeRegistrationSetupResolver)
        {
            _cmsConfig = cmsConfig;
            _typeRegistrationSetupResolver = typeRegistrationSetupResolver;
        }

        IPageSetup ISetupResolver<IPageSetup>.ResolveSetup()
        {
            throw new InvalidOperationException("Cannot resolve page without alias.");
        }

        IPageSetup ISetupResolver<IPageSetup>.ResolveSetup(string alias)
        {
            if (_cache.TryGetValue(alias, out var pageSetup))
            {
                return pageSetup;
            }

            var config = _cmsConfig.CollectionsAndPages.SelectNotNull(x => x as IPageConfig).FirstOrDefault(x => x.Alias == alias);
            if (config == null)
            {
                throw new InvalidOperationException($"Cannot find page with alias {alias}.");
            }

            pageSetup = new PageRegistrationSetup
            {
                Name = config.Name,
                Alias = config.Alias,
                Icon = config.Icon,
                Sections = _typeRegistrationSetupResolver.ResolveSetup(config.SectionRegistrations).ToList()
            };

            // pages always allow caching
            _cache[alias] = pageSetup;

            return pageSetup;
        }
    }
}

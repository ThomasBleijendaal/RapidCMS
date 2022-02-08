using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class PageSetupResolver : ISetupResolver<PageSetup>
    {
        private readonly ICmsConfig _cmsConfig;
        private readonly ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig> _typeRegistrationSetupResolver;
        private readonly Dictionary<string, PageSetup> _cache = new Dictionary<string, PageSetup>();

        public PageSetupResolver(
            ICmsConfig cmsConfig,
            ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig> typeRegistrationSetupResolver)
        {
            _cmsConfig = cmsConfig;
            _typeRegistrationSetupResolver = typeRegistrationSetupResolver;
        }

        Task<PageSetup> ISetupResolver<PageSetup>.ResolveSetupAsync()
        {
            throw new InvalidOperationException("Cannot resolve page without alias.");
        }

        async Task<PageSetup> ISetupResolver<PageSetup>.ResolveSetupAsync(string alias)
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

            var cacheable = true;

            pageSetup = new PageSetup
            {
                Name = config.Name,
                Alias = config.Alias,
                Icon = config.Icon,
                Color = config.Color,
                Sections = (await _typeRegistrationSetupResolver.ResolveSetupAsync(config.SectionRegistrations)).CheckIfCachable(ref cacheable).ToList()
            };

            if (cacheable)
            {
                _cache[alias] = pageSetup;
            }

            return pageSetup;
        }
    }
}

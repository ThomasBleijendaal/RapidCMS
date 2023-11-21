using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup;

internal class PageSetupResolver : ISetupResolver<PageSetup>
{
    private readonly ICmsConfig _cmsConfig;
    private readonly ISetupResolver<TypeRegistrationSetup, CustomTypeRegistrationConfig> _typeRegistrationSetupResolver;
    private Dictionary<string, PageConfig> _pageMap { get; set; } = new Dictionary<string, PageConfig>();
    private readonly Dictionary<string, PageSetup> _cache = new Dictionary<string, PageSetup>();

    public PageSetupResolver(
        ICmsConfig cmsConfig,
        ISetupResolver<TypeRegistrationSetup, CustomTypeRegistrationConfig> typeRegistrationSetupResolver)
    {
        _cmsConfig = cmsConfig;
        _typeRegistrationSetupResolver = typeRegistrationSetupResolver;

        Initialize();
    }

    private void Initialize()
    {
        MapPages(_cmsConfig.CollectionsAndPages);

        void MapPages(IEnumerable<ITreeElementConfig> elements)
        {
            foreach (var page in elements.OfType<PageConfig>())
            {
                if (!_pageMap.TryAdd(page.Alias, page))
                {
                    throw new InvalidOperationException($"Duplicate page alias '{page.Alias}' not allowed.");
                }
            }

            foreach (var collection in elements.OfType<CollectionConfig>().Where(col => col is not ReferencedCollectionConfig))
            {
                var subElements = collection.CollectionsAndPages;
                if (subElements.Any())
                {
                    MapPages(subElements);
                }
            }
        }
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

        if (!_pageMap.TryGetValue(alias, out var config))
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

using System;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers.Setup;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class PageSetupResolver : ISetupResolver<IPageSetup>
    {
        private readonly ICmsConfig _cmsConfig;

        public PageSetupResolver(ICmsConfig cmsConfig)
        {
            _cmsConfig = cmsConfig;
        }

        IPageSetup ISetupResolver<IPageSetup>.ResolveSetup()
        {
            throw new InvalidOperationException("Cannot resolve page without alias.");
        }

        IPageSetup ISetupResolver<IPageSetup>.ResolveSetup(string alias)
        {
            var pageConfig = _cmsConfig.CollectionsAndPages.SelectNotNull(x => x as IPageConfig).FirstOrDefault(x => x.Alias == alias);

            return new PageRegistrationSetup(pageConfig ?? throw new InvalidOperationException($"Cannot find page with alias {alias}."));
        }
    }
}

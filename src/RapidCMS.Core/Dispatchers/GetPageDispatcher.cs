using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Dispatchers
{
    internal class GetPageDispatcher : IPresentationDispatcher<string, IEnumerable<ITypeRegistration>>
    {
        private readonly ISetupResolver<IPageSetup> _pageResolver;

        public GetPageDispatcher(ISetupResolver<IPageSetup> pageResolver)
        {
            _pageResolver = pageResolver;
        }

        public Task<IEnumerable<ITypeRegistration>> GetAsync(string request)
        {
            return Task.FromResult(_pageResolver.ResolveSetup(request).Sections.AsEnumerable());
        }
    }
}

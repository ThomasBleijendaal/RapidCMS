using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Dispatchers
{
    internal class GetPageDispatcher : IPresenationDispatcher<string, IEnumerable<ITypeRegistration>>
    {
        private readonly ICollectionResolver _collectionResolver;

        public GetPageDispatcher(ICollectionResolver collectionResolver)
        {
            _collectionResolver = collectionResolver;
        }

        public Task<IEnumerable<ITypeRegistration>> GetAsync(string request)
        {
            return Task.FromResult(_collectionResolver.GetPage(request).Sections.AsEnumerable());
        }
    }
}

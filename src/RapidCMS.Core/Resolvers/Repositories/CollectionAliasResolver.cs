using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Resolvers;

namespace RapidCMS.Core.Resolvers.Repositories
{
    internal class CollectionAliasResolver : ICollectionAliasResolver
    {
        private readonly IReadOnlyDictionary<string, List<string>> _aliases;

        public CollectionAliasResolver(IReadOnlyDictionary<string, List<string>> aliases)
        {
            _aliases = aliases;
        }

        public IReadOnlyList<string> GetAlias(string repositoryAlias)
            => _aliases[repositoryAlias];
    }
}

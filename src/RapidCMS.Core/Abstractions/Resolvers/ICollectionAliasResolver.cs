using System.Collections.Generic;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface ICollectionAliasResolver
    {
        public IReadOnlyList<string> GetAlias(string repositoryAlias);
    }
}

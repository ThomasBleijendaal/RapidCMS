using System;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Repositories
{
    internal class RepositoryContext : IRepositoryContext
    {
        public RepositoryContext(string? collectionAlias)
        {
            CollectionAlias = collectionAlias;
        }

        public string? CollectionAlias { get; private set; }
    }
}

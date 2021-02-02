using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Resolvers.Repositories
{
    internal class ApiRepositoryTypeResolver : RepositoryTypeResolverBase, IRepositoryTypeResolver
    {
        private readonly IReadOnlyDictionary<string, Type> _repositoryTypes;

        public ApiRepositoryTypeResolver(IApiConfig apiConfig)
        {
            _repositoryTypes = apiConfig.Repositories.ToDictionary(repo => repo.Alias, repo => repo.RepositoryType);
        }

        public Type GetType(string repositoryAlias) 
            => _repositoryTypes.TryGetValue(repositoryAlias, out var type) 
                ? type 
                : throw new InvalidOperationException($"Cannot find Repository for alias '{repositoryAlias}'. Make sure you register the concrete repository class and not an abstract or base class.");

        public (Type entityType, Type databaseType) GetEntityTypes(string repositoryAlias)
            => !_repositoryTypes.TryGetValue(repositoryAlias, out var type) ? throw new InvalidOperationException($"Cannot find Repository for alias '{repositoryAlias}'.")
                : GetGenericParametersOfBaseType(type, typeof(BaseRepository<>)) is Type[] entityType && entityType.Length == 1 ? (entityType[0], entityType[0])
                : GetGenericParametersOfBaseType(type, typeof(BaseMappedRepository<,>)) is Type[] entityTypes && entityTypes.Length == 2 ? (entityTypes[0], entityTypes[1])
                : throw new InvalidOperationException($"Cannot find entity types for Repository '{repositoryAlias}'.");

        // TODO: fix this
        // the API does not handle repositories that were registered under their base class / interface etc.
        public string GetAlias(Type originallyRegisterdType) => throw new InvalidOperationException("Use AliasHelper.GetRepositoryAlias in Api context.");
    }
}

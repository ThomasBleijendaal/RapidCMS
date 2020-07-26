using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;

namespace RapidCMS.Core.Resolvers.Repositories
{
    internal class ApiRepositoryTypeResolver : IRepositoryTypeResolver
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

        // the API does not handle repositories that were registered under their base class / interface etc.
        public string GetAlias(Type originallyRegisterdType) => throw new InvalidOperationException("Use AliasHelper.GetRepositoryAlias in Api context.");
    }
}

using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Resolvers;

namespace RapidCMS.Core.Resolvers.Repositories
{
    internal class CmsRepositoryTypeResolver : IRepositoryTypeResolver
    {
        private readonly IReadOnlyDictionary<string, Type> _repositoryTypes;
        private readonly IReadOnlyDictionary<Type, string> _originallyRegisterdRepositoriesToAlias;

        public CmsRepositoryTypeResolver(IReadOnlyDictionary<string, Type> types, IReadOnlyDictionary<Type, string> originals)
        {
            _repositoryTypes = types;
            _originallyRegisterdRepositoriesToAlias = originals;
        }

        public Type GetType(string repositoryAlias) 
            => _repositoryTypes.TryGetValue(repositoryAlias, out var type) 
                ? type 
                : throw new InvalidOperationException($"Cannot find Repository for alias '{repositoryAlias}'.");

        public string GetAlias(Type originallyRegisteredType)
            => _originallyRegisterdRepositoriesToAlias.TryGetValue(originallyRegisteredType, out var alias)
                    ? alias
                    : throw new InvalidOperationException($"Cannot find alias for Repository '{originallyRegisteredType}'.");
    }
}

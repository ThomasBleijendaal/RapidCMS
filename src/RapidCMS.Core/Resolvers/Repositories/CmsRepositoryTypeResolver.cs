using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Resolvers.Repositories
{
    internal class CmsRepositoryTypeResolver : RepositoryTypeResolverBase, IRepositoryTypeResolver
    {
        private readonly IReadOnlyDictionary<string, Type> _repositoryTypes;
        private readonly IReadOnlyDictionary<Type, string> _originallyRegisterdRepositoriesToAlias;
        private readonly IEnumerable<IPlugin> _plugins;

        public CmsRepositoryTypeResolver(
            IReadOnlyDictionary<string, Type> types,
            IReadOnlyDictionary<Type, string> originals,
            IEnumerable<IPlugin> plugins)
        {
            _repositoryTypes = types;
            _originallyRegisterdRepositoriesToAlias = originals;
            _plugins = plugins;
        }

        public Type GetType(string repositoryAlias)
            => _repositoryTypes.TryGetValue(repositoryAlias, out var type)
                ? type
                : repositoryAlias.Contains("::") && 
                        repositoryAlias.Split("::") is string[] aliasParts && 
                        _plugins.FirstOrDefault(x => x.CollectionPrefix == aliasParts[0]) is IPlugin plugin && 
                        plugin.GetRepository(aliasParts[1]) is Type repositoryType
                    ? repositoryType
                    : throw new InvalidOperationException($"Cannot find Repository for alias '{repositoryAlias}'.");

        public string GetAlias(Type originallyRegisteredType)
            => _originallyRegisterdRepositoriesToAlias.TryGetValue(originallyRegisteredType, out var alias)
                    ? alias
                    : throw new InvalidOperationException($"Cannot find alias for Repository '{originallyRegisteredType}'.");

        public (Type entityType, Type databaseType) GetEntityTypes(string repositoryAlias)
            => !_repositoryTypes.TryGetValue(repositoryAlias, out var type) ? throw new InvalidOperationException($"Cannot find Repository for alias '{repositoryAlias}'.")
                : GetGenericParametersOfBaseType(type, typeof(BaseRepository<>)) is Type[] entityType && entityType.Length == 1 ? (entityType[0], entityType[0])
                : GetGenericParametersOfBaseType(type, typeof(BaseMappedRepository<,>)) is Type[] entityTypes && entityTypes.Length == 2 ? (entityTypes[0], entityTypes[1])
                : throw new InvalidOperationException($"Cannot find entity types for Repository '{repositoryAlias}'.");
    }
}

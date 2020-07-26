using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Resolvers.Repositories
{
    internal class CmsRepositoryTypeResolver : IRepositoryTypeResolver
    {
        private readonly IReadOnlyDictionary<string, Type> _repositoryTypes;
        private readonly IReadOnlyDictionary<Type, string> _originallyRegisterdRepositoriesToAlias;

        public CmsRepositoryTypeResolver(ICmsConfig cmsConfig, IServiceProvider serviceProvider)
        {
            using var repositoryResolvingScope = serviceProvider.CreateScope();

            var types = cmsConfig.RepositoryTypes.Distinct();

            _repositoryTypes = types.ToDictionary(
                type => AliasHelper.GetRepositoryAlias(repositoryResolvingScope.ServiceProvider.GetRequiredService(type).GetType()));

            _originallyRegisterdRepositoriesToAlias = _repositoryTypes.ToDictionary(x => x.Value, x => x.Key);
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

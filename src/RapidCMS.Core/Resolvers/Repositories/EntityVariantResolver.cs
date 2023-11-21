using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Resolvers;

namespace RapidCMS.Core.Resolvers.Repositories;

internal class EntityVariantResolver : IEntityVariantResolver
{
    private readonly IReadOnlyDictionary<string, (Type baseType, IReadOnlyList<Type> derivedTypes)> _entityVariants;

    public EntityVariantResolver(IReadOnlyDictionary<string, (Type baseType, IReadOnlyList<Type> derivedTypes)> entityVariants)
    {
        _entityVariants = entityVariants;
    }

    public (Type? baseType, IReadOnlyList<Type>? derivedTypes) GetValidVariantsForRepository(string repositoryAlias)
        => _entityVariants.TryGetValue(repositoryAlias, out var value) ? value : default;
}

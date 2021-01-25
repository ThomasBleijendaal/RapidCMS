using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface IEntityVariantResolver
    {
        (Type? baseType, IReadOnlyList<Type>? derivedTypes) GetValidVariantsForRepository(string repositoryAlias);
    }
}

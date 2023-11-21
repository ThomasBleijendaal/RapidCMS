using System;

namespace RapidCMS.Core.Resolvers.Repositories;

internal class RepositoryTypeResolverBase
{
    protected Type[]? GetGenericParametersOfBaseType(Type type, Type baseTypeToFind)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == baseTypeToFind)
        {
            return type.GetGenericArguments();
        }
        else if (type.BaseType != null)
        {
            return GetGenericParametersOfBaseType(type.BaseType, baseTypeToFind);
        }
        else
        {
            return null;
        }
    }
}

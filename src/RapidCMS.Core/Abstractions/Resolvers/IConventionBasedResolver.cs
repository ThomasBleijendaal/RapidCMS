using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface IConventionBasedResolver<T>
    {
        T ResolveByConvention(Type subject, Features features);
    }
}

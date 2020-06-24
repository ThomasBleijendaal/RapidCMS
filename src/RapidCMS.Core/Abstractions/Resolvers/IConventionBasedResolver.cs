using System;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IConventionBasedResolver<T>
    {
        T ResolveByConvention(Type subject, Features features, ICollectionSetup? collection);
    }
}

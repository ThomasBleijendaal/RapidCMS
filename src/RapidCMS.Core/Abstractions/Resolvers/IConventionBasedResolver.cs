using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface IConventionBasedResolver<T>
    {
        Task<T> ResolveByConventionAsync(Type subject, Features features, ICollectionSetup? collection);
    }
}

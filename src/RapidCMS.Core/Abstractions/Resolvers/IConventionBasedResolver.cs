using System;
using System.Threading.Tasks;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers;

public interface IConventionBasedResolver<T>
{
    Task<T> ResolveByConventionAsync(Type subject, Features features, CollectionSetup? collection);
}

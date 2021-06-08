using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Factories
{
    public interface IUIResolverFactory
    {
        Task<INodeUIResolver> GetNodeUIResolverAsync(UsageType usageType, string collectionAlias);
        Task<IListUIResolver> GetListUIResolverAsync(UsageType usageType, string collectionAlias);

        Task<INodeUIResolver> GetConventionNodeUIResolverAsync(Type model);
    }
}

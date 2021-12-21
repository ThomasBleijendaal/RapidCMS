using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Factories
{
    public interface IUIResolverFactory
    {
        // TODO: accept CurrentNavigationState
        Task<INodeUIResolver> GetNodeUIResolverAsync(UsageType usageType, string collectionAlias);

        // TODO: accept CurrentNavigationState
        Task<IListUIResolver> GetListUIResolverAsync(UsageType usageType, string collectionAlias);

        Task<INodeUIResolver> GetConventionNodeUIResolverAsync(Type model);
    }
}

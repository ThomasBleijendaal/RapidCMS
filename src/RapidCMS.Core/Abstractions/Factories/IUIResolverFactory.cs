using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Abstractions.Factories;

public interface IUIResolverFactory
{
    Task<INodeUIResolver> GetNodeUIResolverAsync(NavigationState navigationState);

    Task<IListUIResolver> GetListUIResolverAsync(NavigationState navigationState);

    Task<INodeUIResolver> GetConventionNodeUIResolverAsync(Type model);
}

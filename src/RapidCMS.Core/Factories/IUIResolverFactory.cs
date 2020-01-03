using System.Threading.Tasks;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Resolvers.UI;

namespace RapidCMS.Core.Factories
{
    public interface IUIResolverFactory
    {
        Task<INodeUIResolver> GetNodeUIResolverAsync(UsageType usageType, string collectionAlias);
        Task<IListUIResolver> GetListUIResolverAsync(UsageType usageType, string collectionAlias);
    }
}

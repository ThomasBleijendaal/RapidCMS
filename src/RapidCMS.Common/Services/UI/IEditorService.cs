using System.Threading.Tasks;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Resolvers.UI;

namespace RapidCMS.Common.Services.UI
{
    public interface IEditorService
    {
        Task<INodeUIResolver> GetNodeUIResolverAsync(UsageType usageType, string collectionAlias);
        Task<IListUIResolver> GetListUIResolverAsync(UsageType usageType, string collectionAlias);
    }
}

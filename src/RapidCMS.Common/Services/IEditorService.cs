using System.Threading.Tasks;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Services
{
    public interface IEditorService
    {
        Task<NodeUI> GetNodeAsync(UsageType usageType, string collectionAlias);
        Task<ListUI> GetListAsync(UsageType usageType, string collectionAlias);
    }
}

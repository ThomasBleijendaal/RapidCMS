using System.Threading.Tasks;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Abstractions.Services;

public interface ITreeService
{
    Task<TreeCollectionUI?> GetCollectionAsync(string alias, ParentPath? parentPath);
    Task<TreePageUI?> GetPageAsync(string alias, ParentPath? parentPath);
    Task<TreeNodesUI?> GetNodesAsync(string alias, ParentPath? parentPath, int pageNr, int pageSize);
    Task<TreeRootUI> GetRootAsync();
}

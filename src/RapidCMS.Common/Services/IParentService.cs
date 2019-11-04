using System.Threading.Tasks;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Services
{
    internal interface IParentService
    {
        Task<IParent?> GetParentAsync(ParentPath? parentPath);
    }
}

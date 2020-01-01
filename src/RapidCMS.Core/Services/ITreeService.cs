using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Services
{
    public interface ITreeService
    {
        Task<TreeUI?> GetTreeAsync(string alias, ParentPath? parentPath);
        Task<List<TreeNodeUI>> GetNodesAsync(string alias, ParentPath? parentPath);
        IDisposable SubscribeToUpdates(string alias, Func<Task> asyncCallback);
        TreeRootUI GetRoot();
    }
}

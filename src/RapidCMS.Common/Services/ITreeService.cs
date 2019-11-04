using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Services
{
    public interface ITreeService
    {
        Task<TreeUI?> GetTreeAsync(string alias, ParentPath? parentPath);
        Task<List<TreeNodeUI>> GetNodesAsync(string alias, ParentPath? parentPath);
        IDisposable SubscribeToUpdates(string alias, Func<Task> asyncCallback);
        TreeRootUI GetRoot();
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Services
{
    public interface ITreeService
    {
        Task<TreeUI?> GetTreeAsync(string alias, string? parentId);
        Task<List<TreeNodeUI>> GetNodesAsync(string alias, string? parentId);
        IDisposable SubscribeToUpdates(string alias, Func<Task> asyncCallback);
        TreeRootUI GetRoot();
    }
}

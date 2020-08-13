using System;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IRepositoryEventService
    {
        IDisposable SubscribeToRepositoryUpdates(string alias, Func<Task> asyncCallback);
    }
}

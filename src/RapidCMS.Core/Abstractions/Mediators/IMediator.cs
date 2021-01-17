using System;
using System.Threading.Tasks;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Abstractions.Mediators
{
    public interface IMediator
    {
        void NotifyEvent(object sender, IMediatorEventArgs @event);

        event EventHandler<IMediatorEventArgs> OnEvent;

        IDisposable RegisterCallback<TMediatorEventArgs>(Func<object, TMediatorEventArgs, Task> callback, ParentPath? filter)
            where TMediatorEventArgs : IMediatorEventArgs;

        TMediatorEventArgs? GetLatestEventArgs<TMediatorEventArgs>()
             where TMediatorEventArgs : IMediatorEventArgs;
    }
}

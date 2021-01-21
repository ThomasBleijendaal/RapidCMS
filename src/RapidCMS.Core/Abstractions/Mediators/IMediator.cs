using System;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Mediators
{
    public interface IMediator
    {
        void NotifyEvent(object sender, IMediatorEventArgs @event);

        Task<TResponse> NotifyEventAsync<TResponse>(object sender, IMediatorRequestEventArgs<TResponse> @event);

        event EventHandler<IMediatorEventArgs> OnEvent;

        IDisposable RegisterCallback<TMediatorEventArgs>(Func<object, TMediatorEventArgs, Task> callback)
            where TMediatorEventArgs : IMediatorEventArgs;

        TMediatorEventArgs? GetLatestEventArgs<TMediatorEventArgs>()
             where TMediatorEventArgs : IMediatorEventArgs;
    }
}

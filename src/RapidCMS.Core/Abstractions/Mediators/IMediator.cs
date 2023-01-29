using System;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Mediators
{
    public interface IMediator
    {
        Task<TResponse> SendAsync< TResponse>(IRequest<TResponse> request);

        void NotifyEvent(object sender, IMediatorEventArgs @event);

        Task<TResponse> NotifyEventAsync<TResponse>(object sender, IMediatorRequestEventArgs<TResponse> @event);

        event EventHandler<IMediatorEventArgs> OnEvent;

        IDisposable RegisterCallback<TMediatorEventArgs>(Func<object, TMediatorEventArgs, Task> callback)
            where TMediatorEventArgs : IMediatorEventArgs;

        TMediatorEventArgs? GetLatestEventArgs<TMediatorEventArgs>()
             where TMediatorEventArgs : IMediatorEventArgs;
    }
}

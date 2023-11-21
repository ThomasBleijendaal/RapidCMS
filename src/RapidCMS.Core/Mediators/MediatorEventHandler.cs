using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Core.Mediators;

public class MediatorEventHandler<TResponse>
{
    private readonly TaskCompletionSource<TResponse> _tcs = new TaskCompletionSource<TResponse>();
    private readonly IDisposable _responseHandle;
    private readonly IMediator _mediator;
    private readonly Guid _handlerId = Guid.NewGuid();

    public MediatorEventHandler(IMediator mediator)
    {
        _responseHandle = mediator.RegisterCallback<IMediatorResponseEventArgs<TResponse>>(HandleEventCallbackAsync);
        _mediator = mediator;
    }

    public Task<TResponse> HandleEventAsync(IMediatorRequestEventArgs<TResponse> @event)
    {
        @event.RequestId = _handlerId;

        _mediator.NotifyEvent(this, @event);
        return _tcs.Task;
    }

    private Task HandleEventCallbackAsync(object sender, IMediatorResponseEventArgs<TResponse> @event)
    {
        if (@event.RequestId == _handlerId)
        {
            _responseHandle.Dispose();
            _tcs.SetResult(@event.Response);
        }

        return Task.CompletedTask;
    }
}

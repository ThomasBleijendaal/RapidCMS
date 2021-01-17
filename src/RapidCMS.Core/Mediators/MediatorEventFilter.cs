using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Core.Mediators
{
    internal class MediatorEventFilter<TMediatorEventArgs> : IDisposable
        where TMediatorEventArgs : IMediatorEventArgs
    {
        private readonly IMediator _mediator;
        private readonly Func<object, TMediatorEventArgs, Task> _callback;
        private readonly string? _parentFilter;

        public MediatorEventFilter(IMediator mediator, Func<object, TMediatorEventArgs, Task> callback, string? parentFilter)
        {
            _mediator = mediator;
            _callback = callback;
            _parentFilter = parentFilter;
            _mediator.OnEvent += Mediator_OnEventAsync;
        }

        private async void Mediator_OnEventAsync(object? sender, IMediatorEventArgs e)
        {
            if (e is TMediatorEventArgs typedEventArgs && (_parentFilter == null || e.ParentPath?.ToPathString() == _parentFilter))
            {
                await _callback.Invoke(sender ?? _mediator, typedEventArgs);
            }
        }

        public void Dispose()
        {
            _mediator.OnEvent -= Mediator_OnEventAsync;
        }
    }
}

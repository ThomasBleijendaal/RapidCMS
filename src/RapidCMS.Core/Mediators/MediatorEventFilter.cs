using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Core.Mediators
{
    internal class MediatorEventFilter<TMediatorEventArgs> : IDisposable
        where TMediatorEventArgs : IMediatorEventArgs
    {
        private readonly IMediator _mediator;
        private readonly Func<object, TMediatorEventArgs, Task> _callback;

        public MediatorEventFilter(IMediator mediator, Func<object, TMediatorEventArgs, Task> callback)
        {
            _mediator = mediator;
            _callback = callback;
            _mediator.OnEvent += Mediator_OnEventAsync;
        }

        private async void Mediator_OnEventAsync(object? sender, IMediatorEventArgs e)
        {
            if (e is TMediatorEventArgs typedEventArgs)
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

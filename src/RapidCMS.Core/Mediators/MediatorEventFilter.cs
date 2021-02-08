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
        private readonly ILogger<Mediator> _logger;

        public MediatorEventFilter(IMediator mediator, Func<object, TMediatorEventArgs, Task> callback, ILogger<Mediator> logger)
        {
            _mediator = mediator;
            _callback = callback;
            _logger = logger;
            _mediator.OnEvent += Mediator_OnEventAsync;
        }

        private async void Mediator_OnEventAsync(object? sender, IMediatorEventArgs e)
        {
            if (e is TMediatorEventArgs typedEventArgs)
            {
                _logger.LogWarning($"{sender?.GetType()} triggered {typedEventArgs.GetType()}.");
                await _callback.Invoke(sender ?? _mediator, typedEventArgs);
            }
        }

        public void Dispose()
        {
            _mediator.OnEvent -= Mediator_OnEventAsync;
        }
    }
}

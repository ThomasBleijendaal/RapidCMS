using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Core.Mediators
{
    internal class Mediator : IMediator, IDisposable
    {
        private readonly Dictionary<Type, IMediatorEventArgs> _cache = new();
        private readonly IEnumerable<IMediatorEventConverter> _eventConverters;
        private bool _disposedValue;

        public Mediator(IEnumerable<IMediatorEventConverter> eventConverters)
        {
            _eventConverters = eventConverters;
            foreach (var converter in _eventConverters)
            {
                converter.RegisterConversion(this);
            }
        }

        public event EventHandler<IMediatorEventArgs>? OnEvent;

        public void NotifyEvent(object sender, IMediatorEventArgs @event)
        {
            _cache[@event.GetType()] = @event;

            OnEvent?.Invoke(sender, @event);
        }

        public Task<TResponse> NotifyEventAsync<TResponse>(object sender, IMediatorRequestEventArgs<TResponse> @event)
        {
            var handler = new MediatorEventHandler<TResponse>(this);

            return handler.HandleEventAsync(@event);
        }

        public IDisposable RegisterCallback<TMediatorEventArgs>(Func<object, TMediatorEventArgs, Task> callback)
                where TMediatorEventArgs : IMediatorEventArgs
            => new MediatorEventFilter<TMediatorEventArgs>(this, callback);

        public TMediatorEventArgs? GetLatestEventArgs<TMediatorEventArgs>() where TMediatorEventArgs : IMediatorEventArgs
            => _cache.TryGetValue(typeof(TMediatorEventArgs), out var args) ? (TMediatorEventArgs)args : default;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var converter in _eventConverters)
                    {
                        converter.Dispose();
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

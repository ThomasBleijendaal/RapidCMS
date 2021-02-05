using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Core.Mediators
{
    internal class Mediator : IMediator, IDisposable
    {
        private readonly Dictionary<Type, IMediatorEventArgs> _cache = new();
        private readonly IEnumerable<IMediatorEventListener> _listeners;
        private bool _disposedValue;

        public Mediator(IEnumerable<IMediatorEventListener> listeners)
        {
            _listeners = listeners;
            foreach (var listener in _listeners)
            {
                listener.RegisterListener(this);
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
                    foreach (var listener in _listeners)
                    {
                        listener.Dispose();
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

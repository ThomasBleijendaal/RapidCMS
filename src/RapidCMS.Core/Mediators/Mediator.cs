using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Mediators
{
    internal class Mediator : IMediator
    {
        private readonly Dictionary<Type, IMediatorEventArgs> _cache = new();

        public event EventHandler<IMediatorEventArgs>? OnEvent;

        public void NotifyEvent(object sender, IMediatorEventArgs @event)
        {
            _cache[@event.GetType()] = @event;

            OnEvent?.Invoke(sender, @event);
        }

        public IDisposable RegisterCallback<TMediatorEventArgs>(Func<object, TMediatorEventArgs, Task> callback, ParentPath? filter) where TMediatorEventArgs : IMediatorEventArgs
            => new MediatorEventFilter<TMediatorEventArgs>(this, callback, filter?.ToPathString());

        public TMediatorEventArgs? GetLatestEventArgs<TMediatorEventArgs>() where TMediatorEventArgs : IMediatorEventArgs
            => _cache.TryGetValue(typeof(TMediatorEventArgs), out var args) ? (TMediatorEventArgs)args : default;
    }
}

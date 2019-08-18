using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace RapidCMS.Common.Data
{
    public class RepositoryChangeToken : IChangeToken
    {
        private readonly List<RepositoryChangeTokenCallback> _callbacks = new List<RepositoryChangeTokenCallback>();

        private bool _hasChanged = false;

        public bool HasChanged
        {
            get => _hasChanged;
            internal set
            {
                if (value)
                {
                    _hasChanged = true;

                    _callbacks.ForEach(x => x.Invoke().Dispose());
                    _callbacks.Clear();
                }
            }
        }

        public bool ActiveChangeCallbacks => true;
        
        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            var tokenCallback = new RepositoryChangeTokenCallback(callback, state);
            _callbacks.Add(tokenCallback);

            return tokenCallback;
        }

        private class RepositoryChangeTokenCallback : IDisposable
        {
            private Action<object> _callback;
            private object _state;

            public RepositoryChangeTokenCallback(Action<object> callback, object? state)
            {
                _callback = callback ?? throw new ArgumentNullException(nameof(callback));
                _state = state!;
            }

            public void Dispose()
            {
                _callback = null;
                _state = null;
            }

            public RepositoryChangeTokenCallback Invoke()
            {
                _callback.Invoke(_state);

                return this;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace RapidCMS.Core.ChangeToken
{
    public class CmsChangeToken : IChangeToken
    {
        private readonly List<ChangeTokenCallback> _callbacks = new List<ChangeTokenCallback>();

        private bool _hasChanged = false;

        public bool HasChanged
        {
            get => _hasChanged;
            internal set
            {
                if (value)
                {
                    _hasChanged = true;

                    // get a local copy since invoking these callbacks will change the enumerable
                    _callbacks.ToList().ForEach(x => x.Invoke().Dispose());
                    _callbacks.Clear();
                }
            }
        }

        public bool ActiveChangeCallbacks => true;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            var tokenCallback = new ChangeTokenCallback(callback, state);
            _callbacks.Add(tokenCallback);

            return tokenCallback;
        }

        private class ChangeTokenCallback : IDisposable
        {
            private Action<object>? _callback;
            private object? _state;

            public ChangeTokenCallback(Action<object> callback, object? state)
            {
                _callback = callback ?? throw new ArgumentNullException(nameof(callback));
                _state = state;
            }

            public void Dispose()
            {
                _callback = null;
                _state = null;
            }

            public ChangeTokenCallback Invoke()
            {
                _callback?.Invoke(_state!);

                return this;
            }
        }
    }
}

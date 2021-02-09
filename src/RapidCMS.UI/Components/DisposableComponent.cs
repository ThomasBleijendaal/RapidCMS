using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace RapidCMS.UI.Components
{
    public class DisposableComponent : ComponentBase, IDisposable
    {
        private List<IDisposable>? _disposables = new List<IDisposable>();

        protected void DisposeWhenDisposing(IDisposable disposable)
        {
            if (_disposables == null)
            {
                throw new ObjectDisposedException(nameof(disposable));
            }

            _disposables.Add(disposable);
        }

        void IDisposable.Dispose()
        {
            Dispose();

            _disposables?.ForEach(x => x.Dispose());
            _disposables = null;
        }

        protected virtual void Dispose()
        {

        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (_disposables == null)
            {
                throw new ObjectDisposedException($"{nameof(OnInitialized)} called on disposed {nameof(DisposableComponent)}.");
            }

            _disposables.ForEach(x => x.Dispose());
            _disposables.Clear();
        }
    }
}

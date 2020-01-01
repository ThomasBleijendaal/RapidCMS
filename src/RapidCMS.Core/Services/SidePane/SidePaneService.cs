using System;
using System.Threading.Tasks;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Services.SidePane
{
    internal class SidePaneService : ISidePaneService
    {
        private TaskCompletionSource<CrudType> _tcs = new TaskCompletionSource<CrudType>();

        public event EventHandler<PaneEventArgs>? OnPaneRequested;
        public event EventHandler? OnPaneFinished;

        public Task<CrudType> HandlePaneAsync(Type pane, EditContext editContext, ButtonContext buttonContext, CrudType? defaultCrudType)
        {
            var args = new PaneEventArgs(pane, _tcs.Task, editContext, buttonContext, defaultCrudType);

            OnPaneRequested?.Invoke(null, args);

            return _tcs.Task;
        }

        public void PaneHandled(CrudType crudType)
        {
            _tcs.SetResult(crudType);

            OnPaneFinished?.Invoke(null, new EventArgs());

            // reset tcs
            _tcs = new TaskCompletionSource<CrudType>();
        }
    }
}

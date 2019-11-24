using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Services;

namespace RapidCMS.UI.Components.Shared
{
    public abstract class NodesBase : ComponentBase, IDisposable
    {
        private IDisposable? _eventHandle;

        [Inject] protected ITreeService TreeService { get; set; }

        [Parameter] public string CollectionAlias { get; set; }

        [Parameter] public ParentPath? ParentPath { get; set; } = null;

        protected override void OnInitialized()
        {
            OnNodesUpdate();
        }

        private void OnNodesUpdate()
        {
            _eventHandle?.Dispose();
            _eventHandle = TreeService.SubscribeToUpdates(CollectionAlias, async () =>
            {
                await InvokeAsync(() => OnNodesUpdateAsync());
                OnNodesUpdate();
            });
        }

        protected abstract Task OnNodesUpdateAsync();

        public void Dispose()
        {
            _eventHandle?.Dispose();
        }
    }
}

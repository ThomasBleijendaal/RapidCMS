using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.UI.Components.Shared
{
    public abstract class NodesBase : ComponentBase, IDisposable
    {
        private IDisposable? _nodeEventHandle;

        [Inject] protected ITreeService TreeService { get; set; } = default!;
        [Inject] protected IRepositoryEventService RepositoryEventService { get; set; } = default!;
        [Inject] protected IPageState PageState { get; set; } = default!;

        [Parameter] public string CollectionAlias { get; set; } = default!;

        [Parameter] public ParentPath? ParentPath { get; set; } = null;

        protected override void OnInitialized()
        {
            SetupOnNodesUpdate();
        }

        private void SetupOnNodesUpdate()
        {
            _nodeEventHandle?.Dispose();
            _nodeEventHandle = RepositoryEventService.SubscribeToRepositoryUpdates(CollectionAlias, async () =>
            {
                await InvokeAsync(() => OnNodesUpdateAsync());

                SetupOnNodesUpdate();
            });
        }

        protected abstract Task OnNodesUpdateAsync();

        public void Dispose()
        {
            _nodeEventHandle?.Dispose();
        }
    }
}

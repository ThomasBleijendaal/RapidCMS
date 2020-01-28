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
        //private IDisposable? _navEventHandle;

        [Inject] protected ITreeService TreeService { get; set; } = default!;
        [Inject] protected IPageState PageState { get; set; } = default!;

        [Parameter] public string CollectionAlias { get; set; } = default!;

        [Parameter] public ParentPath? ParentPath { get; set; } = null;

        protected override void OnInitialized()
        {
            OnNodesUpdate();
            //OnNavUpdate();
        }

        private void OnNodesUpdate()
        {
            _nodeEventHandle?.Dispose();
            _nodeEventHandle = TreeService.SubscribeToRepositoryUpdates(CollectionAlias, async () =>
            {
                await InvokeAsync(() => OnNodesUpdateAsync());
                OnNodesUpdate();
            });
        }

        //private void OnNavUpdate()
        //{
        //    _navEventHandle?.Dispose();
        //    _navEventHandle = PageState.ChangeToken.RegisterChangeCallback(async (object state) =>
        //    {
        //        await InvokeAsync(() => StateHasChanged());
        //        OnNavUpdate();
        //    }, default);
        //}

        protected abstract Task OnNodesUpdateAsync();

        public void Dispose()
        {
            _nodeEventHandle?.Dispose();
            //_navEventHandle?.Dispose();
        }
    }
}

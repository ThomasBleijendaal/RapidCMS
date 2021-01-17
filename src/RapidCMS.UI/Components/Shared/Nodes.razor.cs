﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Shared
{
    public partial class Nodes
    {
        private const int PageSize = 25;
        private int _pageNr = 1;

        private Dictionary<string, bool>? CollectionsVisible { get; set; }
        private TreeNodesUI? UI { get; set; }
        private string? Error { get; set; }
        private bool Loading { get; set; }

        [Inject] private ITreeService TreeService { get; set; } = default!;
        [Inject] private IRepositoryEventService RepositoryEventService { get; set; } = default!;
        [Inject] private IMediator Mediator { get; set; } = default!;

        [Parameter] public string CollectionAlias { get; set; } = default!;

        [Parameter] public ParentPath? ParentPath { get; set; } = null;

        protected override void OnInitialized()
        {
            DisposeWhenDisposing(Mediator.RegisterCallback<NavigationEventArgs>(LocationChangedAsync, default));
        }

        protected override async Task OnParametersSetAsync()
        {
            await OnNodesUpdateAsync();

            if (Mediator.GetLatestEventArgs<NavigationEventArgs>() is NavigationEventArgs @event)
            {
                await LocationChangedAsync(this, @event);
            }
        }

        //private void SetupOnNodesUpdate()
        //{
        //    _nodeEventHandle?.Dispose();
        //    _nodeEventHandle = RepositoryEventService.SubscribeToRepositoryUpdates(CollectionAlias, async () =>
        //    {
        //        await InvokeAsync(() => OnNodesUpdateAsync());

        //        SetupOnNodesUpdate();
        //    });
        //}

        private async Task LocationChangedAsync(object sender, NavigationEventArgs args)
        {
            if (sender is NavigationLink || !args.ForceUpdate || UI == null || CollectionsVisible == null)
            {
                return;
            }

            foreach (var node in UI.Nodes)
            {
                if (ParentPath.IsBaseOf(args.State.ParentPath, node.RepositoryAlias, node.Id))
                {
                    await InvokeAsync(() =>
                    {
                        CollectionsVisible[node.Id] = true;
                        StateHasChanged();
                    });
                }
            }
        }

        private async Task OnPageChangeAsync(int delta)
        {
            Loading = true;
            _pageNr += delta;
            await OnNodesUpdateAsync();
            Loading = false;
        }

        private async Task OnNodesUpdateAsync()
        {
            var oldNodesVisible = CollectionsVisible;

            try
            {
                UI = await TreeService.GetNodesAsync(CollectionAlias, ParentPath, _pageNr, PageSize);
                if (UI == null)
                {
                    return;
                }

                CollectionsVisible = UI.Nodes.ToDictionary(x => x.Id, x => x.DefaultOpenCollections);

                // restore the view state with the new nodes
                if (oldNodesVisible != null)
                {
                    foreach (var node in oldNodesVisible.Where(x => x.Value))
                    {
                        if (CollectionsVisible.ContainsKey(node.Key))
                        {
                            CollectionsVisible[node.Key] = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            StateHasChanged();
        }
    }
}

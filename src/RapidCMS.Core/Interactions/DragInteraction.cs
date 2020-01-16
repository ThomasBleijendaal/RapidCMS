using System;//
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Models.EventArgs;

namespace RapidCMS.Core.Interactions
{
    // TODO: do not allow dropping elements from collection a to collection b

    internal class DragInteraction : IDragInteraction
    {
        public event EventHandler<RowDragEventArgs>? DragCompletion;
        public event EventHandler<EventArgs>? DragStateChange;

        private string? _draggedId;
        private string? _draggedOverId;
        private Guid? _endZoneGuid;
        private readonly IJSRuntime _js;

        public DragInteraction(IJSRuntime js)
        {
            _js = js;
            _js.InvokeVoidAsync("console.log", "HI");
        }

        public void DragStart(DragEventArgs args, string id)
        {
            _draggedId = id;

            _draggedOverId = default;
            _endZoneGuid = default;

            _js.InvokeVoidAsync("console.log", $"DragStart {id} || {_draggedOverId} {_endZoneGuid}");

            args.DataTransfer.EffectAllowed = "move";
        }

        public void DragEnter(DragEventArgs args, string id)
        {
            if (id == _draggedId)
            {
                return;
            }

            _draggedOverId = id;
            _endZoneGuid = default;

            _js.InvokeVoidAsync("console.log", $"DragEnter {id} || {_draggedOverId} {_endZoneGuid}");

            DragStateChange?.Invoke(default, new EventArgs());
        }

        public void EndZoneDragEnter(DragEventArgs args, Guid guid)
        {
            _js.InvokeVoidAsync("console.log", $"EndZoneDragEnterAsync {guid} || {_draggedOverId} {_endZoneGuid}");

            _draggedOverId = default;
            _endZoneGuid = guid;

            DragStateChange?.Invoke(default, new EventArgs());
        }

        public void DragEnd(DragEventArgs args)
        {
            _js.InvokeVoidAsync("console.log", $"DragEnd {_draggedId} || {_draggedOverId} {_endZoneGuid}");

            if (!string.IsNullOrWhiteSpace(_draggedId) && !string.IsNullOrWhiteSpace(_draggedOverId))
            {
                DragCompletion?.Invoke(default, new RowDragEventArgs(_draggedId, _draggedOverId));
            }
            else if (!string.IsNullOrWhiteSpace(_draggedId) && _endZoneGuid != null)
            {
                DragCompletion?.Invoke(default, new RowDragEventArgs(_draggedId, default));
            }

            _draggedId = default;
            _draggedOverId = default;
            _endZoneGuid = default;

            DragStateChange?.Invoke(default, new EventArgs());
        }

        public bool IsDraggedOver(string id)
        {
            return _draggedOverId == id;
        }

        public bool IsDragged(string id)
        {
            return _draggedId == id;
        }

        public bool IsDraggedOverEndZone(Guid guid)
        {
            return _endZoneGuid == guid;
        }
    }
}

using System;
using Microsoft.AspNetCore.Components.Web;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Models.EventArgs;

namespace RapidCMS.Core.Interactions;

internal class DragInteraction : IDragInteraction
{
    public event EventHandler<RowDragEventArgs>? DragCompletion;
    public event EventHandler<EventArgs>? DragStateChange;

    private string? _draggedId;
    private string? _draggedOverId;
    private Guid? _endZoneGuid;

    public void DragStart(DragEventArgs args, string id)
    {
        _draggedId = id;

        _draggedOverId = default;
        _endZoneGuid = default;

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

        DragStateChange?.Invoke(default, new EventArgs());
    }

    public void EndZoneDragEnter(DragEventArgs args, Guid guid)
    {
        _draggedOverId = default;
        _endZoneGuid = guid;

        DragStateChange?.Invoke(default, new EventArgs());
    }

    public void DragEnd(DragEventArgs args)
    {
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

    public bool IsDraggedOver(string? id)
    {
        return id != null && _draggedOverId == id;
    }

    public bool IsDragged(string? id)
    {
        return id != null && _draggedId == id;
    }

    public bool IsDraggedOverEndZone(Guid guid)
    {
        return _endZoneGuid == guid;
    }
}

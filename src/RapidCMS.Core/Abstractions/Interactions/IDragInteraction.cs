using System;
using Microsoft.AspNetCore.Components.Web;
using RapidCMS.Core.Models.EventArgs;

namespace RapidCMS.Core.Abstractions.Interactions
{
    [Obsolete]
    public interface IDragInteraction
    {
        void DragStart(DragEventArgs args, string id);
        void DragEnd(DragEventArgs args);

        void DragEnter(DragEventArgs args, string id);

        void EndZoneDragEnter(DragEventArgs args, Guid guid);

        bool IsDragged(string? id);
        bool IsDraggedOver(string? id);
        bool IsDraggedOverEndZone(Guid guid);

        event EventHandler<RowDragEventArgs> DragCompletion;
        event EventHandler<EventArgs> DragStateChange;
    }
}

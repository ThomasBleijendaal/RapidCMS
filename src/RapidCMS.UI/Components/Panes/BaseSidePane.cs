using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs.Mediators;

namespace RapidCMS.UI.Components.Panes;

public abstract class BaseSidePane : ComponentBase
{
    [Inject] private IMediator Mediator { get; set; } = default!;

    [Parameter] public FormEditContext? EditContext { get; set; }
    [Parameter] public ButtonContext ButtonContext { get; set; } = default!;
    [Parameter] public Guid RequestId { get; set; } = default!;

    protected void ButtonClicked(CrudType crudType)
    {
        Mediator.NotifyEvent(this, new PaneResponseEventArgs(RequestId, crudType));
    }
}

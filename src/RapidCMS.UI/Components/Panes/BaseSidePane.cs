using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.UI.Components.Panes
{
    public abstract class BaseSidePane : ComponentBase
    {
        [Inject] private ISidePaneService SidePaneService { get; set; } = default!;

        [Parameter] public FormEditContext EditContext { get; set; } = default!;
        [Parameter] public ButtonContext ButtonContext { get; set; } = default!;
        [Parameter] public CrudType? DefaultCrudType { get; set; }

        protected void ButtonClicked(CrudType crudType)
        {
            SidePaneService.PaneHandled(crudType);
        }
    }
}

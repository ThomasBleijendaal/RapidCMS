using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Services.SidePane;

namespace RapidCMS.UI.Components.Panes
{
    public abstract class BaseSidePane : ComponentBase
    {
        [Inject] private ISidePaneService SidePaneService { get; set; }

        [Parameter] public EditContext EditContext { get; set; }
        [Parameter] public ButtonContext ButtonContext { get; set; }
        [Parameter] public CrudType? DefaultCrudType { get; set; }

        protected void ButtonClicked(CrudType crudType)
        {
            SidePaneService.PaneHandled(crudType);
        }
    }
}

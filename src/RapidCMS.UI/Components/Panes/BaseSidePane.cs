using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.UI.Components.Panes
{
    public abstract class BaseSidePane : ComponentBase
    {
        [Inject] private ISidePaneService SidePaneService { get; set; }

        [Parameter] public FormEditContext EditContext { get; set; }
        [Parameter] public ButtonContext ButtonContext { get; set; }
        [Parameter] public CrudType? DefaultCrudType { get; set; }

        protected void ButtonClicked(CrudType crudType)
        {
            SidePaneService.PaneHandled(crudType);
        }
    }
}

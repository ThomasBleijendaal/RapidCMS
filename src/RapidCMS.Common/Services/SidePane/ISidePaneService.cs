using System;
using System.Threading.Tasks;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Services.SidePane
{
    public interface ISidePaneService
    {
        Task<CrudType> HandlePaneAsync(Type pane, EditContext editContext, ButtonContext buttonContext, CrudType? defaultCrudType);
        void PaneHandled(CrudType crudType);

        event EventHandler<PaneEventArgs> OnPaneRequested;
        event EventHandler OnPaneFinished;
    }
}

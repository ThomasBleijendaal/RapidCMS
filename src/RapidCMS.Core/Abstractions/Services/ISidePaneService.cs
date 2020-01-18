using System;
using System.Threading.Tasks;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface ISidePaneService
    {
        Task<CrudType> HandlePaneAsync(Type pane, EditContext editContext, ButtonContext buttonContext, CrudType? defaultCrudType);
        void PaneHandled(CrudType crudType);

        event EventHandler<PaneEventArgs> OnPaneRequested;
        event EventHandler OnPaneFinished;
    }
}

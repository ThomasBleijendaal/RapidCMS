using System.Threading.Tasks;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
using RapidCMS.Common.Services.SidePane;

namespace RapidCMS.Common.ActionHandlers
{
    internal class OpenPaneButtonActionHandler<TSidePane> : DefaultButtonActionHandler
    {
        private readonly ISidePaneService _sidePaneService;

        public OpenPaneButtonActionHandler(ISidePaneService sidePaneService)
        {
            _sidePaneService = sidePaneService;
        }

        public override Task<CrudType> ButtonClickBeforeRepositoryActionAsync(Button button, EditContext editContext, ButtonContext context)
        {
            return _sidePaneService.HandlePaneAsync(typeof(TSidePane), editContext, context, button.DefaultCrudType);
        }
    }
}

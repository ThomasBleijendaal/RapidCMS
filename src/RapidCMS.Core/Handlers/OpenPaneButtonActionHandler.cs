using System.Threading.Tasks;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Services.SidePane;

namespace RapidCMS.Core.Handlers
{
    internal class OpenPaneButtonActionHandler<TSidePane> : DefaultButtonActionHandler
    {
        private readonly ISidePaneService _sidePaneService;

        public OpenPaneButtonActionHandler(ISidePaneService sidePaneService)
        {
            _sidePaneService = sidePaneService;
        }

        public override Task<CrudType> ButtonClickBeforeRepositoryActionAsync(ButtonSetup button, EditContext editContext, ButtonContext context)
        {
            return _sidePaneService.HandlePaneAsync(typeof(TSidePane), editContext, context, button.DefaultCrudType);
        }
    }
}

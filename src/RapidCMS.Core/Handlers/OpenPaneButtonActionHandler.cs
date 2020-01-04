using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Handlers
{
    internal class OpenPaneButtonActionHandler<TSidePane> : DefaultButtonActionHandler
    {
        private readonly ISidePaneService _sidePaneService;

        public OpenPaneButtonActionHandler(ISidePaneService sidePaneService)
        {
            _sidePaneService = sidePaneService;
        }

        public override Task<CrudType> ButtonClickBeforeRepositoryActionAsync(IButton button, EditContext editContext, ButtonContext context)
        {
            return _sidePaneService.HandlePaneAsync(typeof(TSidePane), editContext, context, button.DefaultCrudType);
        }
    }
}

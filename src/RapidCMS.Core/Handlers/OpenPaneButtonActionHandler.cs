using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Handlers;

public class OpenPaneButtonActionHandler<TSidePane> : DefaultButtonActionHandler
{
    private readonly IMediator _mediator;

    public OpenPaneButtonActionHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override Task<CrudType> ButtonClickBeforeRepositoryActionAsync(ButtonSetup button, FormEditContext editContext, ButtonContext context) 
        => _mediator.NotifyEventAsync(this, new PaneRequestEventArgs(typeof(TSidePane), editContext, context));
}

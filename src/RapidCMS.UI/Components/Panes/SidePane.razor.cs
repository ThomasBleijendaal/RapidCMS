using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Models.EventArgs.Mediators;

namespace RapidCMS.UI.Components.Panes;

public partial class SidePane
{
    [Inject] private IMediator Mediator { get; set; } = default!;

    public RenderFragment? Component { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DisposeWhenDisposing(Mediator.RegisterCallback<PaneRequestEventArgs>(OnPaneRequestedAsync));
        DisposeWhenDisposing(Mediator.RegisterCallback<PaneResponseEventArgs>(OnPaneRespondedAsync));
    }

    private async Task OnPaneRequestedAsync(object sender, PaneRequestEventArgs args)
    {
        await InvokeAsync(() =>
        {
            Component = builder =>
            {
                builder.OpenComponent(1, args.PaneType);
                builder.AddAttribute(2, nameof(BaseSidePane.EditContext), args.EditContext);
                builder.AddAttribute(3, nameof(BaseSidePane.ButtonContext), args.ButtonContext);
                builder.AddAttribute(4, nameof(BaseSidePane.RequestId), args.RequestId);
                builder.CloseComponent();
            };

            StateHasChanged();
        });
    }

    private async Task OnPaneRespondedAsync(object sender, PaneResponseEventArgs args)
    {
        await InvokeAsync(() =>
        {
            Component = null;

            StateHasChanged();
        });
    }
}

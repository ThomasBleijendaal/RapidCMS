using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Sections;

public class BaseEditContextSection : EditContextComponentBase
{
    [Parameter] public SectionUI? Section { get; set; }

    protected override void AttachListener()
    {
        EditContext.OnFieldChanged += EditContext_OnFieldChangedAsync;
    }

    private async void EditContext_OnFieldChangedAsync(object? sender, Core.Forms.FieldChangedEventArgs e)
    {
        await InvokeAsync(() => StateHasChanged());
    }

    protected override void DetachListener()
    {
        EditContext.OnFieldChanged -= EditContext_OnFieldChangedAsync;
    }
}

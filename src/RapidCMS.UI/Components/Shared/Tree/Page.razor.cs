using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Shared.Tree;

public partial class Page
{
    [Parameter] public string PageAlias { get; set; } = default!;
    [Parameter] public ParentPath? ParentPath { get; set; } = null;

    private TreePageUI? UI { get; set; }
    private string? Error { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            UI = await TreeService.GetPageAsync(PageAlias, ParentPath);
        }
        catch (Exception ex)
        {
            UI = null;
            Error = ex.Message;
        }

        StateHasChanged();
    }
}

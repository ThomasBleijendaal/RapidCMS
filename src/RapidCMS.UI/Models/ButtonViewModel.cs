using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.UI.Models;

public class ButtonViewModel
{
    public EventCallback<ButtonClickEventArgs>? OnClick { get; set; }

    public async Task NotifyClickAsync(FormEditContext editContext, object? customData)
    {
        var task = OnClick?.InvokeAsync(new ButtonClickEventArgs
        {
            ViewModel = this,
            EditContext = editContext,
            Data = customData
        });

        if (task != null)
        {
            await task;
        }
    }

    public string Label { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public string ButtonId { get; set; } = default!;

    public bool ShouldConfirm { get; set; }
    public bool IsPrimary { get; set; }
    public Func<object, EntityState, bool> IsVisible { get; set; } = default!;
    public bool RequiresValidForm { get; set; }
}

using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Components.Buttons;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Extensions;

public static class ButtonUIExtensions
{
    public static ButtonViewModel ToViewModel(this ButtonUI button) 
        => new ButtonViewModel
        {
            ButtonId = button.ButtonId,
            Icon = button.Icon ?? "",
            Label = button.Label ?? "",
            ShouldConfirm = button.ShouldConfirm,
            IsPrimary = button.IsPrimary,
            IsVisible = button.IsVisible ?? ((object o, EntityState s) => true),
            RequiresValidForm = button.RequiresValidForm
        };

    public static RenderFragment ToRenderFragment(this ButtonUI button, ButtonViewModel model)
        => builder =>
        {
            builder.OpenComponent(0, button.CustomType ?? typeof(DefaultButton));
            builder.AddAttribute(1, nameof(DefaultButton.Model), model);
            builder.CloseComponent();
        };
}

using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Components.Buttons;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Extensions
{
    public static class ButtonUIExtensions
    {
        public static ButtonViewModel ToViewModel(this ButtonUI button)
        {
            return new ButtonViewModel
            {
                ButtonId = button.ButtonId,
                Icon = button.Icon ?? "",
                Label = button.Label ?? "",
                ShouldConfirm = button.ShouldConfirm,
                IsPrimary = button.IsPrimary,
                RequiresValidForm = button.RequiresValidForm
            };
        }

        public static RenderFragment ToRenderFragment(this ButtonUI button, ButtonViewModel model)
        {
            return builder =>
            {
                builder.OpenComponent(0, button.CustomType ?? typeof(DefaultButton));
                builder.AddAttribute(1, nameof(DefaultButton.Model), model);
                builder.CloseComponent();
            };
        }
    }
}

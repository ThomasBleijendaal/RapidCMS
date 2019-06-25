using RapidCMS.Common.Models.UI;
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
                Icon = button.Icon,
                Label = button.Label,
                ShouldConfirm = button.ShouldConfirm,
                IsPrimary = button.IsPrimary,
                RequiresValidForm = button.RequiresValidForm
            };
        }
    }
}

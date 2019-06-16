using System;
using RapidCMS.Common.Validation;

namespace RapidCMS.UI.Models
{
    public class ButtonViewModel
    {
        public event EventHandler<ButtonClickEventArgs> OnClick;

        public void NotifyClick(EditContext editContext, object? customData)
        {
            OnClick?.Invoke(this, new ButtonClickEventArgs
            {
                EditContext = editContext,
                Data = customData
            });
        }

        public string Label { get; set; }
        public string Icon { get; set; }
        public string ButtonId { get; set; }

        public bool ShouldConfirm { get; set; }
        public bool IsPrimary { get; set; }
        public bool RequiresValidForm { get; set; }
    }

    public class ButtonClickEventArgs
    {
        public EditContext EditContext { get; set; }
        public object? Data { get; set; }
    }
}

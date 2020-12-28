using System;
using RapidCMS.Core.Forms;

namespace RapidCMS.UI.Models
{
    public class ButtonViewModel
    {
        public event EventHandler<ButtonClickEventArgs>? OnClick;

        public void NotifyClick(FormEditContext editContext, object? customData)
        {
            OnClick?.Invoke(this, new ButtonClickEventArgs
            {
                ViewModel = this,
                EditContext = editContext,
                Data = customData
            });
        }

        public string Label { get; set; } = default!;
        public string Icon { get; set; } = default!;
        public string ButtonId { get; set; } = default!;

        public bool ShouldConfirm { get; set; }
        public bool IsPrimary { get; set; }
        public bool RequiresValidForm { get; set; }
    }
}

using System;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class ButtonUI
    {
        internal ButtonUI(ButtonSetup button, EditContext editContext)
        {
            ButtonId = button.ButtonId ?? throw new ArgumentNullException(nameof(button.ButtonId));

            Icon = button.Icon;
            Label = button.Label;
            ShouldConfirm = button.ShouldAskForConfirmation(editContext);
            IsPrimary = button.IsPrimary;
            RequiresValidForm = button.RequiresValidForm(editContext);
            CustomType = button.CustomType;
        }

        public string ButtonId { get; private set; }
        public string? Label { get; private set; }
        public string? Icon { get; private set; }
        public bool ShouldConfirm { get; private set; }
        public bool IsPrimary { get; private set; }
        public bool RequiresValidForm { get; private set; }

        public Type? CustomType { get; private set; }
    }
}

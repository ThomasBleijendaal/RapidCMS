using System;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class ButtonUI
    {
        internal ButtonUI(IButtonActionHandler handler, ButtonSetup button, FormEditContext editContext)
        {
            ButtonId = button.ButtonId ?? throw new ArgumentNullException(nameof(button.ButtonId));

            Icon = button.Icon;
            Label = button.Label;
            ShouldConfirm = handler.ShouldAskForConfirmation(button, editContext);
            IsPrimary = button.IsPrimary;
            RequiresValidForm = handler.RequiresValidForm(button, editContext);
            IsVisible = button.IsVisible;
            CustomType = button.CustomType;
        }

        public string ButtonId { get; private set; }
        public string? Label { get; private set; }
        public string? Icon { get; private set; }
        public bool ShouldConfirm { get; private set; }
        public bool IsPrimary { get; private set; }
        public bool RequiresValidForm { get; private set; }
        public Func<object, EntityState, bool>? IsVisible { get; set; }

        public Type? CustomType { get; private set; }
    }
}

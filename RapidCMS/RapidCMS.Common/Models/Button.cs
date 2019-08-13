using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Models
{
    public sealed class Button
    {
        private readonly Type _buttonHandler;

        internal Button(string buttonId, DefaultButtonType defaultButtonType, string? label, string? icon, bool isPrimary, IEnumerable<Button> buttons, Type buttonHandler, EntityVariant? entityVariant = null, string? alias = null)
        {
            var def = defaultButtonType.GetCustomAttribute<DefaultIconLabelAttribute>();

            ButtonId = buttonId ?? throw new ArgumentNullException(nameof(buttonId));
            Alias = alias;

            DefaultButtonType = defaultButtonType;

            Label = label ?? def?.Label ?? "Button";
            Icon = icon ?? def?.Icon ?? "";
            IsPrimary = isPrimary;

            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
            _buttonHandler = buttonHandler ?? throw new ArgumentNullException(nameof(buttonHandler));
            EntityVariant = entityVariant;
        }

        internal DefaultButtonType DefaultButtonType { get; private set; }

        internal string ButtonId { get; private set; }
        internal string? Alias { get; private set; }

        internal string Label { get; private set; }
        internal string Icon { get; private set; }
        internal bool IsPrimary { get; private set; }

        internal IEnumerable<Button> Buttons { get; private set; }

        // expose this as a method as well (just like below)
        internal EntityVariant? EntityVariant { get; set; }

        internal OperationAuthorizationRequirement GetOperation(EditContext editContext) => GetButtonHandler(editContext).GetOperation(this, editContext);
        internal bool IsCompatible(EditContext editContext) => GetButtonHandler(editContext).IsCompatible(this, editContext);
        internal bool ShouldAskForConfirmation(EditContext editContext) => GetButtonHandler(editContext).ShouldAskForConfirmation(this, editContext);
        internal bool RequiresValidForm(EditContext editContext) => GetButtonHandler(editContext).RequiresValidForm(this, editContext);
        internal Task<CrudType> ButtonClickBeforeRepositoryActionAsync(EditContext editContext, ButtonContext context) => GetButtonHandler(editContext).ButtonClickBeforeRepositoryActionAsync(this, editContext, context);
        internal Task ButtonClickAfterRepositoryActionAsync(EditContext editContext, ButtonContext context) => GetButtonHandler(editContext).ButtonClickAfterRepositoryActionAsync(this, editContext, context);

        private IButtonActionHandler GetButtonHandler(EditContext editContext) => editContext.GetService<IButtonActionHandler>(_buttonHandler);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Interfaces.Handlers;

namespace RapidCMS.Core.Models.Setup
{
    public sealed class ButtonSetup
    {
        private readonly Type _buttonHandler;

        internal ButtonSetup(
            string buttonId,
            DefaultButtonType defaultButtonType,
            string? label,
            string? icon,
            bool isPrimary,
            IEnumerable<ButtonSetup> buttons,
            Type buttonHandler,
            EntityVariantSetup? entityVariant = null,
            Type? customType = null,
            CrudType? defaultCrudType = null)
        {
            var def = defaultButtonType.GetCustomAttribute<DefaultIconLabelAttribute>();

            ButtonId = buttonId ?? throw new ArgumentNullException(nameof(buttonId));
            CustomType = customType;

            DefaultButtonType = defaultButtonType;
            DefaultCrudType = defaultCrudType;

            Label = label ?? def?.Label ?? "Button";
            Icon = icon ?? def?.Icon ?? "";
            IsPrimary = isPrimary;

            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
            _buttonHandler = buttonHandler ?? throw new ArgumentNullException(nameof(buttonHandler));
            EntityVariant = entityVariant;
        }

        internal DefaultButtonType DefaultButtonType { get; private set; }
        internal CrudType? DefaultCrudType { get; private set; }

        internal string ButtonId { get; private set; }
        internal Type? CustomType { get; private set; }

        internal string Label { get; private set; }
        internal string Icon { get; private set; }
        internal bool IsPrimary { get; private set; }

        internal IEnumerable<ButtonSetup> Buttons { get; private set; }

        // expose this as a method as well (just like below)
        internal EntityVariantSetup? EntityVariant { get; set; }

        internal OperationAuthorizationRequirement GetOperation(EditContext editContext) => GetButtonHandler(editContext).GetOperation(this, editContext);
        internal bool IsCompatible(EditContext editContext) => GetButtonHandler(editContext).IsCompatible(this, editContext);
        internal bool ShouldAskForConfirmation(EditContext editContext) => GetButtonHandler(editContext).ShouldAskForConfirmation(this, editContext);
        internal bool RequiresValidForm(EditContext editContext) => GetButtonHandler(editContext).RequiresValidForm(this, editContext);
        internal Task<CrudType> ButtonClickBeforeRepositoryActionAsync(EditContext editContext, ButtonContext context) => GetButtonHandler(editContext).ButtonClickBeforeRepositoryActionAsync(this, editContext, context);
        internal Task ButtonClickAfterRepositoryActionAsync(EditContext editContext, ButtonContext context) => GetButtonHandler(editContext).ButtonClickAfterRepositoryActionAsync(this, editContext, context);

        private IButtonActionHandler GetButtonHandler(EditContext editContext) => editContext.GetService<IButtonActionHandler>(_buttonHandler);
    }
}

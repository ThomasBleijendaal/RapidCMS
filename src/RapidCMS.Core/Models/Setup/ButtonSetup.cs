using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Interfaces.Handlers;
using RapidCMS.Core.Interfaces.Setup;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class ButtonSetup : IButton
    {
        private static readonly IEnumerable<ButtonSetup> EmptySubButtons = Enumerable.Empty<ButtonSetup>();

        private readonly Type _buttonHandler;

        internal ButtonSetup(ButtonConfig button, EntityVariantSetup? baseEntityVariant = default, IEnumerable<EntityVariantSetup>? entityVariants = default)
        {
            var def = (button as DefaultButtonConfig)?.ButtonType.GetCustomAttribute<DefaultIconLabelAttribute>();
            Label = button.Label ?? def?.Label ?? "Button";
            Icon = button.Icon ?? def?.Icon ?? "";
            IsPrimary = button.IsPrimary;

            ButtonId = button.Id ?? throw new ArgumentNullException(nameof(button.Id));
            EntityVariant = baseEntityVariant;

            if (button is DefaultButtonConfig defaultButton)
            {
                if (defaultButton.ButtonType == DefaultButtonType.OpenPane)
                {
                    throw new InvalidOperationException($"An {DefaultButtonType.OpenPane} button is not allowed to be used by DefaultButton");
                }

                DefaultButtonType = defaultButton.ButtonType;

                _buttonHandler = typeof(DefaultButtonActionHandler);

                Buttons = defaultButton.ButtonType == DefaultButtonType.New && entityVariants != null
                    ? entityVariants.ToList(variant =>
                        new ButtonSetup(
                            new DefaultButtonConfig
                            {
                                ButtonType = DefaultButtonType.New,
                                Icon = variant.Icon,
                                Id = $"{button.Id}-{variant.Alias}",
                                IsPrimary = button.IsPrimary,
                                Label = string.Format(button.Label ?? variant.Name, variant.Name)
                            },
                        variant))
                    : EmptySubButtons;
            }
            else if (button is CustomButtonConfig customButton)
            {
                CustomType = customButton.CustomType;
                _buttonHandler = customButton.ActionHandler;
            }
            else if (button is PaneButtonConfig paneButton)
            {
                DefaultCrudType = paneButton.CrudType;
                _buttonHandler = typeof(OpenPaneButtonActionHandler<>).MakeGenericType(paneButton.PaneType);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        internal DefaultButtonType DefaultButtonType { get; private set; }
        internal CrudType? DefaultCrudType { get; private set; }

        internal string ButtonId { get; private set; }
        internal Type? CustomType { get; private set; }

        internal string Label { get; private set; }
        internal string Icon { get; private set; }
        internal bool IsPrimary { get; private set; }

        internal IEnumerable<ButtonSetup> Buttons { get; private set; } = EmptySubButtons;

        internal EntityVariantSetup? EntityVariant { get; set; }

        CrudType? IButton.DefaultCrudType => DefaultCrudType;
        string IButton.Label => Label;
        string IButton.Icon => Icon;
        IEntityVariant? IButton.EntityVariant => EntityVariant;

        internal OperationAuthorizationRequirement GetOperation(EditContext editContext) => GetButtonHandler(editContext).GetOperation(this, editContext);
        internal bool IsCompatible(EditContext editContext) => GetButtonHandler(editContext).IsCompatible(this, editContext);
        internal bool ShouldAskForConfirmation(EditContext editContext) => GetButtonHandler(editContext).ShouldAskForConfirmation(this, editContext);
        internal bool RequiresValidForm(EditContext editContext) => GetButtonHandler(editContext).RequiresValidForm(this, editContext);
        internal Task<CrudType> ButtonClickBeforeRepositoryActionAsync(EditContext editContext, ButtonContext context) => GetButtonHandler(editContext).ButtonClickBeforeRepositoryActionAsync(this, editContext, context);
        internal Task ButtonClickAfterRepositoryActionAsync(EditContext editContext, ButtonContext context) => GetButtonHandler(editContext).ButtonClickAfterRepositoryActionAsync(this, editContext, context);

        private IButtonActionHandler GetButtonHandler(EditContext editContext) => editContext.ServiceProvider.GetService<IButtonActionHandler>(_buttonHandler);
    }
}

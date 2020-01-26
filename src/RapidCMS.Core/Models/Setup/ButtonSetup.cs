using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class ButtonSetup : IButton, IButtonSetup
    {
        private static readonly IEnumerable<ButtonSetup> EmptySubButtons = Enumerable.Empty<ButtonSetup>();

        private readonly Type _buttonHandler;

        public ButtonSetup(ButtonConfig button, EntityVariantSetup? baseEntityVariant = default, IEnumerable<EntityVariantSetup>? entityVariants = default)
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
                DefaultButtonType = DefaultButtonType.OpenPane;
                DefaultCrudType = paneButton.CrudType;
                _buttonHandler = typeof(OpenPaneButtonActionHandler<>).MakeGenericType(paneButton.PaneType);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public DefaultButtonType DefaultButtonType { get; private set; }
        public CrudType? DefaultCrudType { get; private set; }

        public string ButtonId { get; private set; }
        public Type? CustomType { get; private set; }

        public string Label { get; private set; }
        public string Icon { get; private set; }
        public bool IsPrimary { get; private set; }

        public IEnumerable<ButtonSetup> Buttons { get; private set; } = EmptySubButtons;

        public EntityVariantSetup? EntityVariant { get; set; }

        DefaultButtonType IButton.DefaultButtonType => DefaultButtonType;
        CrudType? IButton.DefaultCrudType => DefaultCrudType;
        string IButton.Label => Label;
        string IButton.Icon => Icon;
        IEntityVariant? IButton.EntityVariant => EntityVariant;

        public OperationAuthorizationRequirement GetOperation(EditContext editContext) => GetButtonHandler(editContext).GetOperation(this, editContext);
        public bool IsCompatible(EditContext editContext) => GetButtonHandler(editContext).IsCompatible(this, editContext);
        public bool ShouldAskForConfirmation(EditContext editContext) => GetButtonHandler(editContext).ShouldAskForConfirmation(this, editContext);
        public bool RequiresValidForm(EditContext editContext) => GetButtonHandler(editContext).RequiresValidForm(this, editContext);
        public Task<CrudType> ButtonClickBeforeRepositoryActionAsync(EditContext editContext, ButtonContext context) => GetButtonHandler(editContext).ButtonClickBeforeRepositoryActionAsync(this, editContext, context);
        public Task ButtonClickAfterRepositoryActionAsync(EditContext editContext, ButtonContext context) => GetButtonHandler(editContext).ButtonClickAfterRepositoryActionAsync(this, editContext, context);

        private IButtonActionHandler GetButtonHandler(EditContext editContext) => editContext.ServiceProvider.GetService<IButtonActionHandler>(_buttonHandler);
    }
}

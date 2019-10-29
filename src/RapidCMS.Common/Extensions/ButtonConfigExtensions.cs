using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class ButtonConfigExtensions
    {
        private static readonly IEnumerable<Button> EmptySubButtons = Enumerable.Empty<Button>();

        public static Button ToButton(this ButtonConfig button, IEnumerable<EntityVariant>? entityVariants, EntityVariant baseEntityVariant)
        {
            return button switch
            {
                DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(entityVariants, baseEntityVariant),
                CustomButtonConfig customButton => customButton.ToCustomButton(),
                PaneButtonConfig paneButton => paneButton.ToPaneButton(baseEntityVariant),
                _ => throw new InvalidOperationException()
            };
        }

        public static Button ToButton(this ButtonConfig button)
        {
            return button switch
            {
                DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(null, null),
                CustomButtonConfig customButton => customButton.ToCustomButton(),
                PaneButtonConfig paneButton => paneButton.ToPaneButton(null),
                _ => throw new InvalidOperationException()
            };
        }

        public static Button ToDefaultButton(this DefaultButtonConfig button, IEnumerable<EntityVariant>? entityVariants, EntityVariant? baseEntityVariant)
        {
            if (button.ButtonType == DefaultButtonType.OpenPane)
            {
                throw new InvalidOperationException($"An {DefaultButtonType.OpenPane} button is not allowed to be used by DefaultButton");
            }

            var subButtons = button.ButtonType == DefaultButtonType.New && entityVariants != null
                ? entityVariants.ToList(variant => new Button(
                    $"{button.Id}-{variant.Alias}",
                    DefaultButtonType.New,
                    string.Format(button.Label ?? variant.Name, variant.Name),
                    variant.Icon,
                    button.IsPrimary,
                    EmptySubButtons,
                    typeof(DefaultButtonActionHandler),
                    entityVariant: variant))
                : EmptySubButtons;

            return new Button(
                button.Id,
                button.ButtonType,
                button.Label,
                button.Icon,
                button.IsPrimary,
                subButtons,
                typeof(DefaultButtonActionHandler),
                entityVariant: baseEntityVariant);
        }

        public static Button ToCustomButton(this CustomButtonConfig button)
        {
            return new Button(
                button.Id,
                0,
                button.Label,
                button.Icon,
                button.IsPrimary,
                EmptySubButtons,
                button.ActionHandler,
                customType: button.CustomType);
        }

        public static Button ToPaneButton(this PaneButtonConfig button, EntityVariant? baseEntityVariant)
        {
            return new Button(
                button.Id,
                DefaultButtonType.OpenPane,
                button.Label,
                button.Icon,
                button.IsPrimary,
                EmptySubButtons,
                typeof(OpenPaneButtonActionHandler<>).MakeGenericType(button.PaneType),
                entityVariant: baseEntityVariant,
                defaultCrudType: button.CrudType);
        }
    }
}

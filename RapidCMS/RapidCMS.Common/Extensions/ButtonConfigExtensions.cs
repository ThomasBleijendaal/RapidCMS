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
                _ => throw new InvalidOperationException()
            };
        }

        public static Button ToButton(this ButtonConfig button)
        {
            return button switch
            {
                DefaultButtonConfig defaultButton => defaultButton.ToDefaultButton(null, null),
                CustomButtonConfig customButton => customButton.ToCustomButton(),
                _ => throw new InvalidOperationException()
            };
        }

        public static Button ToDefaultButton(this DefaultButtonConfig button, IEnumerable<EntityVariant>? entityVariants, EntityVariant? baseEntityVariant)
        {
            var subButtons = button.ButtonType == DefaultButtonType.New && entityVariants != null
                ? entityVariants.ToList(variant => new Button(
                    Guid.NewGuid().ToString(), 
                    DefaultButtonType.New, 
                    string.Format(button.Label ?? variant.Name, variant.Name), 
                    variant.Icon, 
                    button.IsPrimary, 
                    EmptySubButtons, 
                    typeof(DefaultButtonActionHandler), 
                    entityVariant: variant))
                : EmptySubButtons;

            return new Button(
                Guid.NewGuid().ToString(), 
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
                Guid.NewGuid().ToString(),
                0,
                button.Label,
                button.Icon,
                button.IsPrimary,
                EmptySubButtons,
                button.ActionHandler,
                alias: button.Alias);
        }
    }
}

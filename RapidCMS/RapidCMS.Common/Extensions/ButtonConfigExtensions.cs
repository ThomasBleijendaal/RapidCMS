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
        private static IEnumerable<Button> _emptySubButtons = Enumerable.Empty<Button>();

        public static Button ToDefaultButton(this DefaultButtonConfig button, IEnumerable<EntityVariant>? entityVariants, EntityVariant baseEntityVariant)
        {
            var subButtons = button.ButtonType == DefaultButtonType.New && entityVariants != null
                ? entityVariants.ToList(variant => new Button(
                    Guid.NewGuid().ToString(), 
                    DefaultButtonType.New, 
                    string.Format(button.Label ?? variant.Name, variant.Name), 
                    variant.Icon, 
                    button.IsPrimary, 
                    _emptySubButtons, 
                    typeof(DefaultButtonActionHandler), 
                    entityVariant: variant))
                : _emptySubButtons;

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
                _emptySubButtons,
                button.ActionHandler,
                alias: button.Alias);
        }
    }
}

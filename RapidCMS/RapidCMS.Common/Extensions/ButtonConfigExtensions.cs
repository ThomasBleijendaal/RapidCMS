using System;
using System.Collections.Generic;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.Services;

#nullable enable

namespace RapidCMS.Common.Extensions
{
    public static class ButtonConfigExtensions
    {
        public static DefaultButton ToDefaultButton(this DefaultButtonConfig button, IEnumerable<EntityVariant>? entityVariants, EntityVariant baseEntityVariant)
        {
            return new DefaultButton
            {
                ButtonId = Guid.NewGuid().ToString(),
                DefaultButtonType = button.ButtonType,
                Icon = button.Icon,
                Label = button.Label,
                Buttons = button.ButtonType == DefaultButtonType.New && entityVariants != null
                ? entityVariants.ToList(variant => new DefaultButton
                {
                    ButtonId = Guid.NewGuid().ToString(),
                    DefaultButtonType = DefaultButtonType.New,
                    Icon = variant.Icon,
                    Label = variant.Name,
                    Metadata = variant
                } as Button)
                : new List<Button>(),
                Metadata = baseEntityVariant,
                ShouldConfirm = button.ButtonType == DefaultButtonType.Delete,
                IsPrimary = button.IsPrimary
            };
        }

        public static CustomButton ToCustomButton(this CustomButtonConfig button)
        {
            var handler = (button.ActionHandler != null)
                ? ServiceLocator.Instance.GetService<IButtonActionHandler>(button.ActionHandler)
                : new DefaultButtonActionHandler(button.CrudType, button.Action);

            return new CustomButton()
            {
                ActionHandler = handler,
                ButtonId = Guid.NewGuid().ToString(),
                Alias = button.Alias,
                Icon = button.Icon,
                Label = button.Label,
                Buttons = new List<Button>(),
                ShouldConfirm = handler.ShouldConfirm(),
                IsPrimary = button.IsPrimary
            };
        }
    }
}

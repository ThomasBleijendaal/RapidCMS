using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Interfaces;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.Services;

namespace RapidCMS.Common.Extensions
{
    public static class ButtonConfigExtensions
    {
        public static DefaultButton ToDefaultButton(this DefaultButtonConfig button, IEnumerable<EntityVariant> entityVariants)
        {
            var variants = entityVariants.Count();

            return new DefaultButton
            {
                ButtonId = Guid.NewGuid().ToString(),
                DefaultButtonType = button.ButtonType,
                Icon = button.Icon,
                Label = button.Label,
                Buttons = button.ButtonType == DefaultButtonType.New && variants > 1
                ? entityVariants.ToList(variant => new DefaultButton
                {
                    ButtonId = Guid.NewGuid().ToString(),
                    DefaultButtonType = DefaultButtonType.New,
                    Icon = variant.Icon,
                    Label = variant.Name,
                    Metadata = variant
                } as Button)
                : new List<Button>(),
                Metadata = variants == 1 ? entityVariants.First() : null,
                ShouldConfirm = button.ButtonType == DefaultButtonType.Delete
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
                ShouldConfirm = handler.ShouldConfirm()
            };
        }
    }
}

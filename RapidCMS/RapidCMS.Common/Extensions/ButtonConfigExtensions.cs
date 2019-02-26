using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    public static class ButtonConfigExtensions
    {
        public static DefaultButton ToDefaultButton(this DefaultButtonConfig button, IEnumerable<EntityVariant> entityVariants)
        {
            return new DefaultButton
            {
                ButtonId = Guid.NewGuid().ToString(),
                DefaultButtonType = button.ButtonType,
                Icon = button.Icon,
                Label = button.Label,
                Buttons = button.ButtonType == DefaultButtonType.New && entityVariants.Count() > 1
                ? entityVariants.ToList(variant => new DefaultButton
                {
                    ButtonId = Guid.NewGuid().ToString(),
                    DefaultButtonType = DefaultButtonType.New,
                    Icon = variant.Icon,
                    Label = variant.Name,
                    Metadata = variant
                } as Button)
                : new List<Button>()
            };
        }

        public static CustomButton ToCustomButton(this CustomButtonConfig button)
        {
            return new CustomButton
            {
                ButtonId = Guid.NewGuid().ToString(),
                Action = button.Action,
                Alias = button.Alias,
                Icon = button.Icon,
                Label = button.Label,
                Buttons = new List<Button>()
            };
        }
    }
}

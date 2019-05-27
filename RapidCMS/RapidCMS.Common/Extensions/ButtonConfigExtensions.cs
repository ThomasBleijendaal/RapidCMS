using System;
using System.Collections.Generic;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

#nullable enable

namespace RapidCMS.Common.Extensions
{
    internal static class ButtonConfigExtensions
    {
        public static Button ToDefaultButton(this DefaultButtonConfig button, IEnumerable<EntityVariant>? entityVariants, EntityVariant baseEntityVariant)
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

        public static Button ToCustomButton(this CustomButtonConfig button, IServiceProvider serviceProvider)
        {
            var handler = (button.ActionHandler != null)
                ? serviceProvider.GetService<IButtonActionHandler>(button.ActionHandler)
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

    internal static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider, Type type)
            where T : class
        {
            return serviceProvider.GetService(type) as T ?? throw new InvalidOperationException($"Failed to resolve instance of type {type} and cast it as {typeof(T)}");
        }
    }
}

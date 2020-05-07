using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Handlers;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Setup
{
    internal class ButtonSetupResolver : ISetupResolver<IButtonSetup, ButtonConfig>
    {
        private static readonly IEnumerable<ButtonSetup> EmptySubButtons = Enumerable.Empty<ButtonSetup>();

        public IButtonSetup ResolveSetup(ButtonConfig config, ICollectionSetup? collection = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var @default = (config as DefaultButtonConfig)?.ButtonType.GetCustomAttribute<DefaultIconLabelAttribute>();

            var button = new ButtonSetup
            {
                Buttons = EmptySubButtons,

                Label = config.Label ?? @default?.Label ?? "Button",
                Icon = config.Icon ?? @default?.Icon ?? "",
                IsPrimary = config.IsPrimary,

                ButtonId = config.Id ?? throw new ArgumentNullException(nameof(config.Id)),
                EntityVariant = collection.EntityVariant
            };

            if (config is DefaultButtonConfig defaultButton)
            {
                if (defaultButton.ButtonType == DefaultButtonType.OpenPane)
                {
                    throw new InvalidOperationException($"An {DefaultButtonType.OpenPane} button is not allowed to be used by DefaultButton");
                }

                button.DefaultButtonType = defaultButton.ButtonType;

                button.ButtonHandlerType = typeof(DefaultButtonActionHandler);

                if (defaultButton.ButtonType == DefaultButtonType.New && collection.SubEntityVariants != null)
                {
                    button.Buttons = collection.SubEntityVariants.ToList(variant =>
                        new ButtonSetup
                        {
                            Label = string.Format(button.Label ?? variant.Name, variant.Name),
                            Icon = variant.Icon ?? @default?.Icon ?? "",
                            IsPrimary = config.IsPrimary,
                            ButtonId = $"{config.Id}-{variant.Alias}",
                            EntityVariant = variant,
                            DefaultButtonType = DefaultButtonType.New,
                            ButtonHandlerType = typeof(DefaultButtonActionHandler),
                            Buttons = EmptySubButtons
                        });
                }
            }
            else if (config is CustomButtonConfig customButton)
            {
                button.CustomType = customButton.CustomType;
                button.ButtonHandlerType = customButton.ActionHandler;
            }
            else if (config is PaneButtonConfig paneButton)
            {
                button.DefaultButtonType = DefaultButtonType.OpenPane;
                button.DefaultCrudType = paneButton.CrudType;
                button.ButtonHandlerType = typeof(OpenPaneButtonActionHandler<>).MakeGenericType(paneButton.PaneType);
            }
            else
            {
                throw new InvalidOperationException();
            }

            return button;
        }
    }
}

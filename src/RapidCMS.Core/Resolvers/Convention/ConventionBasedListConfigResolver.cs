using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Resolvers.Convention
{
    internal class ConventionBasedListConfigResolver : IConventionBasedResolver<ListConfig>
    {
        private readonly IFieldConfigResolver _fieldConfigResolver;
        private readonly ILanguageResolver _languageResolver;

        public ConventionBasedListConfigResolver(
            IFieldConfigResolver fieldConfigResolver,
            ILanguageResolver languageResolver)
        {
            _fieldConfigResolver = fieldConfigResolver;
            _languageResolver = languageResolver;
        }

        public ListConfig ResolveByConvention(Type subject, Features features, ICollectionSetup? collection)
        {
            var listButtons = new List<ButtonConfig>();

            if (features.HasFlag(Features.CanEdit) || features.HasFlag(Features.CanGoToEdit))
            {
                listButtons.Add(new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.New,
                    Label = !(collection?.SubEntityVariants?.Any() ?? false) ? null : _languageResolver.ResolveText("New {0}")
                });
                listButtons.Add(new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.Return
                });
                listButtons.Add(new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.SaveExisting,
                    Label = _languageResolver.ResolveText("Update all")
                });
            };
            var paneButtons = new List<ButtonConfig>();
            if (features.HasFlag(Features.CanGoToView))
            {
                paneButtons.Add(new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.View
                });
            }
            if (features.HasFlag(Features.CanGoToEdit))
            {
                paneButtons.Add(new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.Edit
                });
            }
            if (features.HasFlag(Features.CanEdit))
            {
                paneButtons.Add(new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.SaveExisting
                });
                paneButtons.Add(new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.SaveNew
                });
                paneButtons.Add(new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.Delete
                });
            }

            var result = new ListConfig(subject)
            {
                PageSize = 25,
                Buttons = listButtons,
                ListEditorType = ListType.Table,
                Panes = new List<PaneConfig>
                {
                    new PaneConfig(subject)
                    {
                        Buttons =  paneButtons,
                        FieldIndex = 1,
                        Fields = _fieldConfigResolver.GetFields(subject, features).ToList(),
                        VariantType = subject
                    }
                },
                ReorderingAllowed = false,
                SearchBarVisible = true
            };

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Common.Models.Config
{
    public class ListEditorConfig
    {
        internal ListEditorType ListEditorType { get; set; }
        internal EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<ListEditorPaneConfig> ListEditors { get; set; } = new List<ListEditorPaneConfig>();

    }

    public class ListEditorConfig<TEntity> : ListEditorConfig
        where TEntity : IEntity
    {
        public ListEditorConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
        {
            var button = new DefaultButtonConfig
            {
                ButtonType = type,
                Icon = icon ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Icon,
                Label = label ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Label,
                IsPrimary = isPrimary
            };

            Buttons.Add(button);

            return this;
        }

        public ListEditorConfig<TEntity> AddCustomButton(Type buttonType, CrudType crudType, Action action, string? label = null, string? icon = null)
        {
            var button = new CustomButtonConfig(buttonType)
            {
                Action = action,
                CrudType = crudType,
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public ListEditorConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string label = null, string icon = null)
        {
            var button = new CustomButtonConfig(buttonType)
            {
                ActionHandler = typeof(TActionHandler),
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public ListEditorConfig<TEntity> AddSection(Action<ListEditorPaneConfig<TEntity>> configure)
        {
            return AddSection<TEntity>(configure);
        }

        public ListEditorConfig<TEntity> AddSection(Type customSectionType, Action<ListEditorPaneConfig<TEntity>> configure)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public ListEditorConfig<TEntity> AddSection<TDerivedEntity>(Action<ListEditorPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }


        public ListEditorConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<ListEditorPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null
                ? new ListEditorPaneConfig<TDerivedEntity>(typeof(TDerivedEntity))
                : new ListEditorPaneConfig<TDerivedEntity>(typeof(TDerivedEntity), customSectionType);

            configure.Invoke(config);

            ListEditors.Add(config);

            return this;
        }
    }
}

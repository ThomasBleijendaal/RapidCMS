using System;
using System.Collections.Generic;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Common.Models.Config
{
    public class ListViewConfig
    {
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<ListViewPaneConfig> ListViewPanes { get; set; } = new List<ListViewPaneConfig>();
    }

    public class ListViewConfig<TEntity> : ListViewConfig
        where TEntity : IEntity
    {
        public ListViewConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public ListViewConfig<TEntity> AddCustomButton(Type buttonType, CrudType crudType, Action action, string? label = null, string? icon = null)
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

        public ListViewConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
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

        public ListViewConfig<TEntity> AddListPane(Action<ListViewPaneConfig<TEntity>> configure)
        {
            return AddListPane<TEntity>(configure);
        }

        public ListViewConfig<TEntity> AddListPane<TDerivedEntity>(Action<ListViewPaneConfig<TEntity>> configure)
            where TDerivedEntity : TEntity
        {
            var config = new ListViewPaneConfig<TEntity>();

            configure.Invoke(config);

            config.VariantType = typeof(TDerivedEntity);

            ListViewPanes.Add(config);

            return this;
        }
    }
}

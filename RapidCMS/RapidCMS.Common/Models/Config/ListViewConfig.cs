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
        internal int? PageSize { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<ListViewPaneConfig> ListViewPanes { get; set; } = new List<ListViewPaneConfig>();
    }

    public class ListViewConfig<TEntity> : ListViewConfig
        where TEntity : IEntity
    {
        public ListViewConfig<TEntity> SetPageSize(int pageSize)
        {
            PageSize = pageSize;

            return this;
        }

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

        public ListViewConfig<TEntity> AddRow(Action<ListViewPaneConfig<TEntity>> configure)
        {
            return AddRow<TEntity>(configure);
        }

        public ListViewConfig<TEntity> AddRow(Type customSectionType, Action<ListViewPaneConfig<TEntity>> configure)
        {
            return AddRow<TEntity>(customSectionType, configure);
        }

        public ListViewConfig<TEntity> AddRow<TDerivedEntity>(Action<ListViewPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddRow(null, configure);
        }

        private ListViewConfig<TEntity> AddRow<TDerivedEntity>(Type? customSectionType, Action<ListViewPaneConfig<TDerivedEntity>>? configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null
                ? new ListViewPaneConfig<TDerivedEntity>(typeof(TDerivedEntity))
                : new ListViewPaneConfig<TDerivedEntity>(typeof(TDerivedEntity), customSectionType);

            configure?.Invoke(config);

            ListViewPanes.Add(config);

            return this;
        }
    }
}

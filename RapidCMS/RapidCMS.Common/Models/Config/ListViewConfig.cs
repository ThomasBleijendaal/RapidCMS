using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public class ListViewConfig<TEntity> : ListConfig, IHasButtons<ListViewConfig<TEntity>>
        where TEntity : IEntity
    {
        public ListViewConfig<TEntity> SetPageSize(int pageSize)
        {
            PageSize = pageSize;

            return this;
        }

        public ListViewConfig<TEntity> SetSearchBarVisibility(bool visible)
        {
            SearchBarVisible = visible;

            return this;
        }

        public ListViewConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
        {
            var button = new DefaultButtonConfig
            {
                ButtonType = type,
                Icon = icon,
                Label = label,
                IsPrimary = isPrimary
            };

            Buttons.Add(button);

            return this;
        }

        public ListViewConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
        {
            var button = new CustomButtonConfig(buttonType, typeof(TActionHandler))
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public ListViewConfig<TEntity> AddRow(Action<IDisplayPaneConfig<TEntity>> configure)
        {
            return AddRow<TEntity>(configure);
        }

        public ListViewConfig<TEntity> AddRow(Type customSectionType, Action<IDisplayPaneConfig<TEntity>> configure)
        {
            return AddRow<TEntity>(customSectionType, configure);
        }

        public ListViewConfig<TEntity> AddRow<TDerivedEntity>(Action<IDisplayPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddRow(null, configure);
        }

        private ListViewConfig<TEntity> AddRow<TDerivedEntity>(Type? customSectionType, Action<IDisplayPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null
                ? new PaneConfig<TDerivedEntity, IFieldConfig<TDerivedEntity>>(typeof(TDerivedEntity))
                : new PaneConfig<TDerivedEntity, IFieldConfig<TDerivedEntity>>(typeof(TDerivedEntity), customSectionType);

            configure?.Invoke(config);

            Panes.Add(config);

            return this;
        }
    }
}

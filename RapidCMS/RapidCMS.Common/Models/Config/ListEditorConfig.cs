using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public class ListEditorConfig<TEntity> : ListConfig, IHasButtons<ListEditorConfig<TEntity>>
        where TEntity : IEntity
    {
        public ListEditorConfig<TEntity> SetPageSize(int pageSize)
        {
            PageSize = pageSize;

            return this;
        }

        public ListEditorConfig<TEntity> SetSearchBarVisibility(bool visible)
        {
            SearchBarVisible = visible;

            return this;
        }

        public ListEditorConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public ListEditorConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
        {
            var button = new CustomButtonConfig(buttonType, typeof(TActionHandler))
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public ListEditorConfig<TEntity> AddSection(Action<IEditorPaneConfig<TEntity>> configure)
        {
            return AddSection<TEntity>(configure);
        }

        public ListEditorConfig<TEntity> AddSection(Type customSectionType, Action<IEditorPaneConfig<TEntity>> configure)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public ListEditorConfig<TEntity> AddSection<TDerivedEntity>(Action<IEditorPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }


        public ListEditorConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<IEditorPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null
                ? new PaneConfig<TDerivedEntity, IFieldConfig<TDerivedEntity>>(typeof(TDerivedEntity))
                : new PaneConfig<TDerivedEntity, IFieldConfig<TDerivedEntity>>(typeof(TDerivedEntity), customSectionType);

            configure.Invoke(config);

            Panes.Add(config);

            return this;
        }
    }
}

using System;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config
{
    internal class ListEditorConfig<TEntity> : ListConfig,
        IListEditorConfig<TEntity>
        where TEntity : IEntity
    {
        public IListEditorConfig<TEntity> SetPageSize(int pageSize)
        {
            if (pageSize < 1)
            {
                throw new InvalidOperationException("Cannot have PageSize smaller than 1");
            }

            PageSize = pageSize;

            return this;
        }

        public IListEditorConfig<TEntity> SetSearchBarVisibility(bool visible)
        {
            SearchBarVisible = visible;

            return this;
        }

        public IListEditorConfig<TEntity> AllowReordering(bool allowReordering)
        {
            ReorderingAllowed = allowReordering;

            return this;
        }

        public IListEditorConfig<TEntity> SetListType(ListType listType)
        {
            ListEditorType = listType;

            return this;
        }

        public IListEditorConfig<TEntity> SetColumnVisibility(EmptyVariantColumnVisibility columnVisibility)
        {
            EmptyVariantColumnVisibility = columnVisibility;

            return this;
        }

        public IListEditorConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public IListEditorConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
            where TActionHandler : IButtonActionHandler
        {
            var button = new CustomButtonConfig(buttonType, typeof(TActionHandler))
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public IListEditorConfig<TEntity> AddPaneButton(Type paneType, string? label = null, string? icon = null, CrudType? defaultCrudType = null)
        {
            var button = new PaneButtonConfig(paneType, defaultCrudType)
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public IListEditorConfig<TEntity> AddSection(Action<IEditorPaneConfig<TEntity>> configure)
        {
            return AddSection<TEntity>(configure);
        }

        public IListEditorConfig<TEntity> AddSection(Type customSectionType, Action<IEditorPaneConfig<TEntity>>? configure = null)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public IListEditorConfig<TEntity> AddSection<TDerivedEntity>(Action<IEditorPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }

        public IListEditorConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<IEditorPaneConfig<TDerivedEntity>>? configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null
                ? new PaneConfig<TDerivedEntity>(typeof(TDerivedEntity))
                : new PaneConfig<TDerivedEntity>(typeof(TDerivedEntity), customSectionType);

            configure.Invoke(config);

            Panes.Add(config);

            return this;
        }
    }
}

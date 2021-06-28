﻿using System;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config
{
    internal class ListViewConfig<TEntity> : ListConfig, IListViewConfig<TEntity>
        where TEntity : IEntity
    {
        public ListViewConfig() : base(typeof(TEntity))
        {
        }

        public IListViewConfig<TEntity> SetPageSize(int pageSize)
        {
            PageSize = pageSize;

            return this;
        }

        public IListViewConfig<TEntity> SetSearchBarVisibility(bool visible)
        {
            SearchBarVisible = visible;

            return this;
        }

        public IListViewConfig<TEntity> SetColumnVisibility(EmptyVariantColumnVisibility columnVisibility)
        {
            EmptyVariantColumnVisibility = columnVisibility;

            return this;
        }

        public IListViewConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public IListViewConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
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

        public IListViewConfig<TEntity> AddPaneButton(Type paneType, string? label = null, string? icon = null)
        {
            var button = new PaneButtonConfig(paneType)
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public IListViewConfig<TEntity> AddNavigationButton<TNavigationHandler>(string? label = null, string? icon = null)
            where TNavigationHandler : INavigationHandler
        {
            var button = new NavigationButtonConfig(typeof(TNavigationHandler))
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public IListViewConfig<TEntity> AddRow(Action<IDisplayPaneConfig<TEntity>> configure)
        {
            return AddRow<TEntity>(configure);
        }

        public IListViewConfig<TEntity> AddRow(Type customSectionType, Action<IDisplayPaneConfig<TEntity>> configure)
        {
            return AddRow<TEntity>(customSectionType, configure);
        }

        public IListViewConfig<TEntity> AddRow<TDerivedEntity>(Action<IDisplayPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddRow(null, configure);
        }

        public IListViewConfig<TEntity> AddRow<TDerivedEntity>(Type? customSectionType, Action<IDisplayPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null
                ? new PaneConfig<TDerivedEntity>(typeof(TDerivedEntity))
                : new PaneConfig<TDerivedEntity>(typeof(TDerivedEntity), customSectionType);

            configure?.Invoke(config);

            Panes.Add(config);

            return this;
        }
    }
}

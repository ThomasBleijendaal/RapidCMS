using System;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Config
{
    internal class NodeViewConfig<TEntity> : NodeConfig, INodeViewConfig<TEntity>
        where TEntity : IEntity
    {
        public NodeViewConfig() : base(typeof(TEntity))
        {
        }

        public INodeViewConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false, Func<IEntity, EntityState, bool>? isVisible = null)
        {
            var button = new DefaultButtonConfig
            {
                ButtonType = type,
                Icon = icon,
                Label = label,
                IsPrimary = isPrimary
            };

            button.VisibleWhen(isVisible);

            Buttons.Add(button);

            return this;
        }

        public INodeViewConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null, Func<IEntity, EntityState, bool>? isVisible = null)
            where TActionHandler : IButtonSetupActionHandler
        {
            var button = new CustomButtonConfig(buttonType, typeof(TActionHandler))
            {
                Icon = icon,
                Label = label
            };

            button.VisibleWhen(isVisible);

            Buttons.Add(button);

            return this;
        }

        public INodeViewConfig<TEntity> AddPaneButton(Type paneType, string? label = null, string? icon = null, Func<IEntity, EntityState, bool>? isVisible = null)
        {
            var button = new PaneButtonConfig(paneType)
            {
                Icon = icon,
                Label = label
            };

            button.VisibleWhen(isVisible);

            Buttons.Add(button);

            return this;
        }

        public INodeViewConfig<TEntity> AddNavigationButton<TNavigationHandler>(string? label = null, string? icon = null, Func<IEntity, EntityState, bool>? isVisible = null)
            where TNavigationHandler : INavigationHandler
        {
            var button = new NavigationButtonConfig(typeof(TNavigationHandler))
            {
                Icon = icon,
                Label = label
            };

            button.VisibleWhen(isVisible);

            Buttons.Add(button);

            return this;
        }

        public INodeViewConfig<TEntity> AddSection(Action<IDisplayPaneConfig<TEntity>> configure) 
            => AddSection<TEntity>(configure);

        public INodeViewConfig<TEntity> AddSection(Type customSectionType, Action<IDisplayPaneConfig<TEntity>>? configure = null) 
            => AddSection<TEntity>(customSectionType, configure);

        public INodeViewConfig<TEntity> AddSection<TDerivedEntity>(Action<IDisplayPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity 
            => AddSection(null, configure);

        public INodeViewConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<IDisplayPaneConfig<TDerivedEntity>>? configure)
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

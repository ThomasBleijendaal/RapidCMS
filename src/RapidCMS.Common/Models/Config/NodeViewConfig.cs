using System;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    internal class NodeViewConfig<TEntity> : NodeConfig, INodeViewConfig<TEntity>
        where TEntity : IEntity
    {
        public NodeViewConfig() : base(typeof(TEntity))
        {
        }

        public INodeViewConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public INodeViewConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
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

        public INodeViewConfig<TEntity> AddPaneButton(Type paneType, string? label = null, string? icon = null, CrudType? defaultCrudType = null)
        {
            var button = new PaneButtonConfig(paneType, defaultCrudType)
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public INodeViewConfig<TEntity> AddSection(Action<IDisplayPaneConfig<TEntity>> configure)
        {
            return AddSection<TEntity>(configure);
        }

        public INodeViewConfig<TEntity> AddSection(Type customSectionType, Action<IDisplayPaneConfig<TEntity>>? configure = null)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public INodeViewConfig<TEntity> AddSection<TDerivedEntity>(Action<IDisplayPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }

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

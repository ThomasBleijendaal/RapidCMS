using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public class NodeViewConfig<TEntity> : NodeConfig, IHasButtons<NodeViewConfig<TEntity>>
        where TEntity : IEntity
    {
        public NodeViewConfig() : base(typeof(TEntity))
        {
        }

        public NodeViewConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public NodeViewConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
        {
            var button = new CustomButtonConfig(buttonType, typeof(TActionHandler))
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public NodeViewConfig<TEntity> AddSection(Action<IDisplayPaneConfig<TEntity>> configure)
        {
            return AddSection<TEntity>(configure);
        }

        public NodeViewConfig<TEntity> AddSection(Type customSectionType, Action<IDisplayPaneConfig<TEntity>>? configure = null)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public NodeViewConfig<TEntity> AddSection<TDerivedEntity>(Action<IDisplayPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }

        private NodeViewConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<IDisplayPaneConfig<TDerivedEntity>>? configure)
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

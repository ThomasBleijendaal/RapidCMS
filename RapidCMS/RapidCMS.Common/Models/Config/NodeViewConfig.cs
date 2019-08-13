using System;
using System.Collections.Generic;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Common.Models.Config
{
    public class NodeViewConfig
    {
        public NodeViewConfig(Type baseType)
        {
            BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
        }

        internal Type BaseType { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<NodeViewPaneConfig> ViewPanes { get; set; } = new List<NodeViewPaneConfig>();
    }

    public class NodeViewConfig<TEntity> : NodeViewConfig
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

        public NodeViewConfig<TEntity> AddSection(Action<NodeViewPaneConfig<TEntity>> configure)
        {
            return AddSection<TEntity>(configure);
        }

        public NodeViewConfig<TEntity> AddSection(Type customSectionType, Action<NodeViewPaneConfig<TEntity>>? configure = null)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public NodeViewConfig<TEntity> AddSection<TDerivedEntity>(Action<NodeViewPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }

        private NodeViewConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<NodeViewPaneConfig<TDerivedEntity>>? configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null 
                ? new NodeViewPaneConfig<TDerivedEntity>(typeof(TDerivedEntity)) 
                : new NodeViewPaneConfig<TDerivedEntity>(typeof(TDerivedEntity), customSectionType);

            configure?.Invoke(config);

            ViewPanes.Add(config);

            return this;
        }
    }
}

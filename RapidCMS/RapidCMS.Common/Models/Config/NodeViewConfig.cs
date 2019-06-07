using System;
using System.Collections.Generic;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;

#nullable enable

namespace RapidCMS.Common.Models.Config
{
    public class NodeViewConfig
    {
        internal Type BaseType { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<NodeViewPaneConfig> ViewPanes { get; set; } = new List<NodeViewPaneConfig>();
    }

    public class NodeViewConfig<TEntity> : NodeViewConfig
        where TEntity : IEntity
    {
        public NodeViewConfig()
        {
            BaseType = typeof(TEntity);
        }

        public NodeViewConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null, bool isPrimary = false)
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

        public NodeViewConfig<TEntity> AddCustomButton(Type buttonType, CrudType crudType, Action action, string label = null, string icon = null)
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

        public NodeViewConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string label = null, string icon = null)
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

        public NodeViewConfig<TEntity> AddViewPane(Action<NodeViewPaneConfig<TEntity>> configure)
        {
            return AddViewPane<TEntity>(configure);
        }

        public NodeViewConfig<TEntity> AddViewPane(Type customSectionType, Action<NodeViewPaneConfig<TEntity>>? configure = null)
        {
            return AddViewPane<TEntity>(customSectionType, configure);
        }

        public NodeViewConfig<TEntity> AddViewPane<TDerivedEntity>(Action<NodeViewPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddViewPane(null, configure);
        }

        private NodeViewConfig<TEntity> AddViewPane<TDerivedEntity>(Type? customSectionType, Action<NodeViewPaneConfig<TDerivedEntity>>? configure)
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

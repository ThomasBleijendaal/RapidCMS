using System;
using System.Collections.Generic;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;


namespace RapidCMS.Common.Models.Config
{
    public class NodeEditorConfig
    {
        internal Type BaseType { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<NodeEditorPaneConfig> EditorPanes { get; set; } = new List<NodeEditorPaneConfig>();
    }

    public class NodeEditorConfig<TEntity> : NodeEditorConfig
        where TEntity : IEntity
    {
        public NodeEditorConfig()
        {
            BaseType = typeof(TEntity);
        }

        public NodeEditorConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null, bool isPrimary = false)
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

        public NodeEditorConfig<TEntity> AddCustomButton(Type buttonType, CrudType crudType, Action action, string label = null, string icon = null)
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

        public NodeEditorConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string label = null, string icon = null)
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

        public NodeEditorConfig<TEntity> AddEditorPane(Action<NodeEditorPaneConfig<TEntity>> configure)
        {
            return AddEditorPane<TEntity>(configure);
        }

        public NodeEditorConfig<TEntity> AddEditorPane(Type customSectionType, Action<NodeEditorPaneConfig<TEntity>>? configure = null)
        {
            return AddEditorPane<TEntity>(customSectionType, configure);
        }

        public NodeEditorConfig<TEntity> AddEditorPane<TDerivedEntity>(Action<NodeEditorPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddEditorPane(null, configure);
        }

        private NodeEditorConfig<TEntity> AddEditorPane<TDerivedEntity>(Type? customSectionType, Action<NodeEditorPaneConfig<TDerivedEntity>>? configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null 
                ? new NodeEditorPaneConfig<TDerivedEntity>(typeof(TDerivedEntity)) 
                : new NodeEditorPaneConfig<TDerivedEntity>(typeof(TDerivedEntity), customSectionType);

            configure?.Invoke(config);

            EditorPanes.Add(config);

            return this;
        }
    }
}

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
        public NodeEditorConfig(Type baseType)
        {
            BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
        }

        internal Type BaseType { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<NodeEditorPaneConfig> EditorPanes { get; set; } = new List<NodeEditorPaneConfig>();
    }

    public class NodeEditorConfig<TEntity> : NodeEditorConfig
        where TEntity : IEntity
    {
        public NodeEditorConfig() : base(typeof(TEntity))
        {
        }

        public NodeEditorConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public NodeEditorConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
        {
            var button = new CustomButtonConfig(buttonType, typeof(TActionHandler))
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public NodeEditorConfig<TEntity> AddSection(Action<NodeEditorPaneConfig<TEntity>> configure)
        {
            return AddSection<TEntity>(configure);
        }

        public NodeEditorConfig<TEntity> AddSection(Type customSectionType, Action<NodeEditorPaneConfig<TEntity>>? configure = null)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public NodeEditorConfig<TEntity> AddSection<TDerivedEntity>(Action<NodeEditorPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }

        private NodeEditorConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<NodeEditorPaneConfig<TDerivedEntity>>? configure)
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

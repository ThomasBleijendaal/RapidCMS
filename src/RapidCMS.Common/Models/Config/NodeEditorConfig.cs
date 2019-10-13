using System;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    internal class NodeEditorConfig<TEntity> : NodeConfig, INodeEditorConfig<TEntity>
        where TEntity : IEntity
    {
        public NodeEditorConfig() : base(typeof(TEntity))
        {
        }

        public INodeEditorConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public INodeEditorConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
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

        public INodeEditorConfig<TEntity> AddPaneButton(Type paneType, string? label = null, string? icon = null, CrudType? defaultCrudType = null)
        {
            var button = new PaneButtonConfig(paneType, defaultCrudType)
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public INodeEditorConfig<TEntity> AddSection(Action<IEditorPaneConfig<TEntity>>? configure)
        {
            return AddSection<TEntity>(configure);
        }

        public INodeEditorConfig<TEntity> AddSection(Type customSectionType, Action<IEditorPaneConfig<TEntity>>? configure = null)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public INodeEditorConfig<TEntity> AddSection<TDerivedEntity>(Action<IEditorPaneConfig<TDerivedEntity>>? configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }

        public INodeEditorConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<IEditorPaneConfig<TDerivedEntity>>? configure)
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

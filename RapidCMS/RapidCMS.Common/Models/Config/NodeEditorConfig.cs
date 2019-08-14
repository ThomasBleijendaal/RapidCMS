using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public class NodeEditorConfig<TEntity> : NodeConfig, IHasButtons<NodeEditorConfig<TEntity>>
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

        public NodeEditorConfig<TEntity> AddSection(Action<PaneConfig<TEntity, IEditorFieldConfig<TEntity>>>? configure)
        {
            return AddSection<TEntity>(configure);
        }

        public NodeEditorConfig<TEntity> AddSection(Type customSectionType, Action<PaneConfig<TEntity, IEditorFieldConfig<TEntity>>>? configure = null)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public NodeEditorConfig<TEntity> AddSection<TDerivedEntity>(Action<PaneConfig<TDerivedEntity, IEditorFieldConfig<TDerivedEntity>>>? configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }

        private NodeEditorConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<PaneConfig<TDerivedEntity, IEditorFieldConfig<TDerivedEntity>>>? configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null
                ? new PaneConfig<TDerivedEntity, IEditorFieldConfig<TDerivedEntity>>(typeof(TDerivedEntity))
                : new PaneConfig<TDerivedEntity, IEditorFieldConfig<TDerivedEntity>>(typeof(TDerivedEntity), customSectionType);

            configure?.Invoke(config);

            Panes.Add(config);

            return this;
        }
    }
}

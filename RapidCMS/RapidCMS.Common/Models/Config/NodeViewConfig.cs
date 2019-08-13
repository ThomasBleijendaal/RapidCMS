using System;
using System.Collections.Generic;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

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
        internal List<PaneConfig> ViewPanes { get; set; } = new List<PaneConfig>();
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

        public NodeViewConfig<TEntity> AddSection(Action<PaneConfig<TEntity, IReadonlyFieldConfig<TEntity>>> configure)
        {
            return AddSection<TEntity>(configure);
        }

        public NodeViewConfig<TEntity> AddSection(Type customSectionType, Action<PaneConfig<TEntity, IReadonlyFieldConfig<TEntity>>>? configure = null)
        {
            return AddSection<TEntity>(customSectionType, configure);
        }

        public NodeViewConfig<TEntity> AddSection<TDerivedEntity>(Action<PaneConfig<TDerivedEntity, IReadonlyFieldConfig<TDerivedEntity>>> configure)
            where TDerivedEntity : TEntity
        {
            return AddSection(null, configure);
        }

        private NodeViewConfig<TEntity> AddSection<TDerivedEntity>(Type? customSectionType, Action<PaneConfig<TDerivedEntity, IReadonlyFieldConfig<TDerivedEntity>>>? configure)
            where TDerivedEntity : TEntity
        {
            var config = customSectionType == null
                ? new PaneConfig<TDerivedEntity, IReadonlyFieldConfig<TDerivedEntity>>(typeof(TDerivedEntity))
                : new PaneConfig<TDerivedEntity, IReadonlyFieldConfig<TDerivedEntity>>(typeof(TDerivedEntity), customSectionType);

            configure?.Invoke(config);

            ViewPanes.Add(config);

            return this;
        }
    }
}

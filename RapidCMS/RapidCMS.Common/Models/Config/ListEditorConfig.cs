using System;
using System.Collections.Generic;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Common.Models.Config
{
    public class ListEditorConfig
    {
        public ListEditorType ListEditorType { get; set; }
        public List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        public List<ListEditorPaneConfig> ListEditors { get; set; } = new List<ListEditorPaneConfig>();

    }

    public class ListEditorConfig<TEntity> : ListEditorConfig
        where TEntity : IEntity
    {
        public ListEditorConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null, bool isPrimary = false)
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

        public ListEditorConfig<TEntity> AddEditor(Action<ListEditorPaneConfig<TEntity>> configure)
        {
            return AddEditor<TEntity>(configure);
        }

        // TODO: do not allow for more than one editor of the same EntityType

        public ListEditorConfig<TEntity> AddEditor<TDerivedEntity>(Action<ListEditorPaneConfig<TDerivedEntity>> configure)
            where TDerivedEntity : TEntity
        {
            var config = new ListEditorPaneConfig<TDerivedEntity>();

            configure.Invoke(config);

            config.VariantType = typeof(TDerivedEntity);

            ListEditors.Add(config);

            return this;
        }
    }
}

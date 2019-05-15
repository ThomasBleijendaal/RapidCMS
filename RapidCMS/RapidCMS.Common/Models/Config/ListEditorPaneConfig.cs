using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;

namespace RapidCMS.Common.Models.Config
{
    public class ListEditorPaneConfig
    {
        public Type VariantType { get; set; }
        public List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        public List<FieldConfig> Fields { get; set; } = new List<FieldConfig>();
    }

    public class ListEditorPaneConfig<TEntity> : ListEditorPaneConfig
        where TEntity : IEntity
    {
        public ListEditorPaneConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null, bool isPrimary = false)
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

        public ListEditorPaneConfig<TEntity> AddCustomButton(Type buttonType, CrudType crudType, Action action, string label = null, string icon = null)
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

        public ListEditorPaneConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string label = null, string icon = null)
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

        public FieldConfig<TEntity> AddField<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<FieldConfig<TEntity>> configure = null)
        {
            var config = new FieldConfig<TEntity>()
            {
                // TODO: update
                NodeProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression)
            };
            config.Name = config.NodeProperty.PropertyName;
            config.Type = EditorTypeHelper.TryFindDefaultEditorType(config.NodeProperty.PropertyType);

            configure?.Invoke(config);

            Fields.Add(config);

            return config;
        }
    }
}

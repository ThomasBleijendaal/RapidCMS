using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Interfaces;

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

        public FieldConfig<TEntity> AddField<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<FieldConfig<TEntity>> configure = null)
        {
            var config = new FieldConfig<TEntity>()
            {
                NodeProperty = PropertyMetadataHelper.Create(propertyExpression)
            };
            config.Name = config.NodeProperty.PropertyName;

            // try to find the default editor for this type
            foreach (var type in EnumHelper.GetValues<EditorType>())
            {
                if (type.GetCustomAttribute<DefaultTypeAttribute>()?.Types.Contains(config.NodeProperty.PropertyType) ?? false)
                {
                    config.Type = type;

                    break;
                }
            }

            configure?.Invoke(config);

            Fields.Add(config);

            return config;
        }
    }
}

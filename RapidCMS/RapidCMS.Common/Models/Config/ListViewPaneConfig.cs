using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.Config
{
    public class ListViewPaneConfig
    {
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();

        // TODO: not the best name
        internal List<PropertyConfig> Properties { get; set; } = new List<PropertyConfig>();
    }

    public class ListViewPaneConfig<TEntity> : ListViewPaneConfig
        where TEntity : IEntity
    {
        public ListViewPaneConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null, bool isPrimary = false)
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

        public ListViewPaneConfig<TEntity> AddCustomButton(Type buttonType, CrudType crudType, Action action, string label = null, string icon = null)
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

        public ListViewPaneConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string label = null, string icon = null)
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

        public PropertyConfig<TEntity> AddProperty<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<PropertyConfig<TEntity>> configure = null)
        {
            var config = new PropertyConfig<TEntity>
            {
                Property = PropertyMetadataHelper.GetExpressionMetadata(propertyExpression)
            };
            config.Name = config.Property.PropertyName;

            configure?.Invoke(config);

            Properties.Add(config);

            return config;
        }
    }
}

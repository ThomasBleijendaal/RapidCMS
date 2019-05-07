using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;

namespace RapidCMS.Common.Models.Config
{
    public class ListViewPaneConfig
    {
        public List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        public List<PropertyConfig> Properties { get; set; } = new List<PropertyConfig>();
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

        public ListViewPaneConfig<TEntity> AddCustomButton(string alias, CrudType crudType, Action action, string label = null, string icon = null)
        {
            var button = new CustomButtonConfig(alias)
            {
                Action = action,
                CrudType = crudType,
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public ListViewPaneConfig<TEntity> AddCustomButton<TActionHandler>(string alias, string label = null, string icon = null)
        {
            var button = new CustomButtonConfig(alias)
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
                NodeProperty = PropertyMetadataHelper.Create(propertyExpression)
            };
            config.Name = config.NodeProperty.PropertyName;

            configure?.Invoke(config);

            Properties.Add(config);

            return config;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Interfaces;

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

        public ListViewPaneConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null)
        {
            var button = new DefaultButtonConfig
            {
                ButtonType = type,
                Icon = icon ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Icon,
                Label = label ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Label
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

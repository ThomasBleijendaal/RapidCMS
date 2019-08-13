using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;

namespace RapidCMS.Common.Models.Config
{
    public class ListViewPaneConfig
    {
        protected ListViewPaneConfig(Type variantType)
        {
            VariantType = variantType ?? throw new ArgumentNullException(nameof(variantType));
        }

        internal string? CustomAlias { get; set; }

        internal Func<object, bool> IsVisible { get; set; } = (x) => true;

        internal Type VariantType { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();

        // TODO: not the best name
        internal List<PropertyConfig> Properties { get; set; } = new List<PropertyConfig>();
    }

    public class ListViewPaneConfig<TEntity> : ListViewPaneConfig
        where TEntity : IEntity
    {
        public ListViewPaneConfig(Type variantType) : base(variantType)
        {
        }

        public ListViewPaneConfig(Type variantType, Type customSectionType) : base(variantType)
        {
            CustomAlias = customSectionType.FullName;
        }

        public ListViewPaneConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public ListViewPaneConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
        {
            var button = new CustomButtonConfig(buttonType, typeof(TActionHandler))
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public PropertyConfig<TEntity> AddProperty(Expression<Func<TEntity, string>> propertyExpression, Action<PropertyConfig<TEntity>>? configure = null)
        {
            var config = new PropertyConfig<TEntity>
            {
                Property = PropertyMetadataHelper.GetExpressionMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression))
            };
            config.Name = config.Property.PropertyName;

            configure?.Invoke(config);

            Properties.Add(config);

            return config;
        }

        public ListViewPaneConfig<TEntity> VisibleWhen(Func<TEntity, bool> predicate)
        {
            IsVisible = (entity) => predicate.Invoke((TEntity)entity);

            return this;
        }
    }
}

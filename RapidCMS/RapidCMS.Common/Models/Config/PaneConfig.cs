using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;

namespace RapidCMS.Common.Models.Config
{
    // TODO: add button support
    public abstract class PaneConfig
    {
        protected PaneConfig(Type variantType)
        {
            VariantType = variantType ?? throw new ArgumentNullException(nameof(variantType));
        }

        internal string? CustomAlias { get; set; }
        internal string? Label { get; set; }

        internal Func<object, bool> IsVisible { get; set; } = (x) => true;

        internal Type VariantType { get; set; }

        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<FieldConfig> Fields { get; set; } = new List<FieldConfig>();
        internal List<SubCollectionListConfig> SubCollectionLists { get; set; } = new List<SubCollectionListConfig>();
        internal List<RelatedCollectionListConfig> RelatedCollectionLists { get; set; } = new List<RelatedCollectionListConfig>();
    }

    public class PaneConfig<TEntity, TFieldConfig> : PaneConfig, IHasButtons<PaneConfig<TEntity, TFieldConfig>>
        where TEntity : IEntity
        where TFieldConfig : class, IFieldConfig<TEntity>
    {
        internal int FieldIndex { get; set; } = 0;

        public PaneConfig(Type variantType) : base(variantType)
        {
        }

        public PaneConfig(Type variantType, Type customSectionType) : base(variantType)
        {
            CustomAlias = customSectionType.FullName;
        }

        public PaneConfig<TEntity, TFieldConfig> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        public PaneConfig<TEntity, TFieldConfig> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
        {
            var button = new CustomButtonConfig(buttonType, typeof(TActionHandler))
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public PaneConfig<TEntity, TFieldConfig> SetLabel(string label)
        {
            Label = label;

            return this;
        }

        public TFieldConfig AddField<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<TFieldConfig>? configure = null)
        {
            var config = new FieldConfig<TEntity>()
            {
                Property = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression)),
            };
            config.Name = config.Property.PropertyName;
            config.Type = EditorTypeHelper.TryFindDefaultEditorType(config.Property.PropertyType);

            configure?.Invoke((config as TFieldConfig)!);

            config.Index = FieldIndex++;

            Fields.Add(config);

            return (config as TFieldConfig)!;
        }

        public PaneConfig<TEntity, TFieldConfig> AddSubCollectionList<TSubEntity>(string collectionAlias, Action<SubCollectionListConfig<TSubEntity>>? configure = null)
            where TSubEntity : IEntity
        {
            var config = new SubCollectionListConfig<TSubEntity>(collectionAlias);

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            SubCollectionLists.Add(config);

            return this;
        }

        public PaneConfig<TEntity, TFieldConfig> AddRelatedCollectionList<TRelatedEntity>(string collectionAlias, Action<RelatedCollectionListConfig<TEntity, TRelatedEntity>>? configure = null)
            where TRelatedEntity : IEntity
        {
            var config = new RelatedCollectionListConfig<TEntity, TRelatedEntity>(collectionAlias);

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            RelatedCollectionLists.Add(config);

            return this;
        }

        public PaneConfig<TEntity, TFieldConfig> VisibleWhen(Func<TEntity, bool> predicate)
        {
            IsVisible = (entity) => predicate.Invoke((TEntity)entity);

            return this;
        }
    }
}

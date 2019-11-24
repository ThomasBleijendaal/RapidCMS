using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;

namespace RapidCMS.Common.Models.Config
{
    internal abstract class PaneConfig
    {
        protected PaneConfig(Type variantType)
        {
            VariantType = variantType ?? throw new ArgumentNullException(nameof(variantType));
        }

        internal Type? CustomType { get; set; }
        internal string? Label { get; set; }

        internal Func<object, EntityState, bool> IsVisible { get; set; } = (x, y) => true;

        internal Type VariantType { get; set; }

        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<FieldConfig> Fields { get; set; } = new List<FieldConfig>();
        internal List<SubCollectionListConfig> SubCollectionLists { get; set; } = new List<SubCollectionListConfig>();
        internal List<RelatedCollectionListConfig> RelatedCollectionLists { get; set; } = new List<RelatedCollectionListConfig>();
    }

    internal class PaneConfig<TEntity> : PaneConfig, IDisplayPaneConfig<TEntity>, IEditorPaneConfig<TEntity>
        where TEntity : IEntity
    {
        internal int FieldIndex { get; set; } = 0;

        internal PaneConfig(Type variantType) : base(variantType)
        {
        }

        internal PaneConfig(Type variantType, Type customSectionType) : base(variantType)
        {
            CustomType = customSectionType;
        }

        private PaneConfig<TEntity> AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false)
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

        private PaneConfig<TEntity> AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null)
        {
            var button = new CustomButtonConfig(buttonType, typeof(TActionHandler))
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        private PaneConfig<TEntity> AddPaneButton(Type paneType, string? label = null, string? icon = null, CrudType? defaultCrudType = null)
        {
            var button = new PaneButtonConfig(paneType, defaultCrudType)
            {
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        private PaneConfig<TEntity> SetLabel(string label)
        {
            Label = label;

            return this;
        }

        private SubCollectionListConfig<TSubEntity> AddSubCollectionList<TSubEntity>(string collectionAlias, Action<SubCollectionListConfig<TSubEntity>>? configure = null)
            where TSubEntity : IEntity
        {
            var config = new SubCollectionListConfig<TSubEntity>(collectionAlias);

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            SubCollectionLists.Add(config);

            return config;
        }

        private RelatedCollectionListConfig<TEntity, TRelatedEntity> AddRelatedCollectionList<TRelatedEntity>(string collectionAlias, Action<RelatedCollectionListConfig<TEntity, TRelatedEntity>>? configure = null)
            where TRelatedEntity : IEntity
        {
            var config = new RelatedCollectionListConfig<TEntity, TRelatedEntity>(collectionAlias);

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            RelatedCollectionLists.Add(config);

            return config;
        }

        private PaneConfig<TEntity> VisibleWhen(Func<TEntity, EntityState, bool> predicate)
        {
            IsVisible = (entity, state) => predicate.Invoke((TEntity)entity, state);

            return this;
        }

        IDisplayFieldConfig<TEntity, string> IDisplayPaneConfig<TEntity>.AddField(Expression<Func<TEntity, string?>> displayExpression, Action<IDisplayFieldConfig<TEntity, string>>? configure)
        {
            var config = new FieldConfig<TEntity, string>()
            {
                Expression = PropertyMetadataHelper.GetExpressionMetadata(displayExpression) ?? throw new InvalidPropertyExpressionException(nameof(displayExpression)),
            };
            config.Name = config.Expression.PropertyName;
            config.DisplayType = default;

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            Fields.Add(config);

            return config;
        }

        IDisplayPaneConfig<TEntity> IHasButtons<IDisplayPaneConfig<TEntity>>.AddDefaultButton(DefaultButtonType type, string? label, string? icon, bool isPrimary)
        {
            return AddDefaultButton(type, label, icon, isPrimary);
        }

        IDisplayPaneConfig<TEntity> IHasButtons<IDisplayPaneConfig<TEntity>>.AddCustomButton<TActionHandler>(Type buttonType, string? label, string? icon)
        {
            return AddCustomButton<TActionHandler>(buttonType, label, icon);
        }

        IDisplayPaneConfig<TEntity> IHasButtons<IDisplayPaneConfig<TEntity>>.AddPaneButton(Type paneType, string? label, string? icon, CrudType? defaultCrudType)
        {
            return AddPaneButton(paneType, label, icon, defaultCrudType);
        }

        ISubCollectionListConfig<TSubEntity> IDisplayPaneConfig<TEntity>.AddSubCollectionList<TSubEntity>(string collectionAlias, Action<ISubCollectionListConfig<TSubEntity>>? configure)
        {
            return AddSubCollectionList(collectionAlias, configure);
        }

        IRelatedCollectionListConfig<TEntity, TRelatedEntity> IDisplayPaneConfig<TEntity>.AddRelatedCollectionList<TRelatedEntity>(string collectionAlias, Action<IRelatedCollectionListConfig<TEntity, TRelatedEntity>>? configure)
        {
            return AddRelatedCollectionList(collectionAlias, configure);
        }

        IDisplayPaneConfig<TEntity> IDisplayPaneConfig<TEntity>.SetLabel(string label)
        {
            return SetLabel(label);
        }

        IDisplayPaneConfig<TEntity> IDisplayPaneConfig<TEntity>.VisibleWhen(Func<TEntity, EntityState, bool> predicate)
        {
            return VisibleWhen(predicate);
        }

        IEditorFieldConfig<TEntity, TValue> IEditorPaneConfig<TEntity>.AddField<TValue>(Expression<Func<TEntity, TValue>> propertyExpression, Action<IEditorFieldConfig<TEntity, TValue>>? configure)
        {
            var config = new FieldConfig<TEntity, TValue>()
            {
                Property = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression) ?? throw new InvalidPropertyExpressionException(nameof(propertyExpression)),
            };
            config.Name = config.Property.PropertyName;
            config.EditorType = EditorTypeHelper.TryFindDefaultEditorType(config.Property.PropertyType);

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            Fields.Add(config);

            return config;
        }

        IEditorPaneConfig<TEntity> IHasButtons<IEditorPaneConfig<TEntity>>.AddDefaultButton(DefaultButtonType type, string? label, string? icon, bool isPrimary)
        {
            return AddDefaultButton(type, label, icon, isPrimary);
        }

        IEditorPaneConfig<TEntity> IHasButtons<IEditorPaneConfig<TEntity>>.AddCustomButton<TActionHandler>(Type buttonType, string? label, string? icon)
        {
            return AddCustomButton<TActionHandler>(buttonType, label, icon);
        }

        IEditorPaneConfig<TEntity> IHasButtons<IEditorPaneConfig<TEntity>>.AddPaneButton(Type paneType, string? label, string? icon, CrudType? defaultCrudType)
        {
            return AddPaneButton(paneType, label, icon, defaultCrudType);
        }

        ISubCollectionListConfig<TSubEntity> IEditorPaneConfig<TEntity>.AddSubCollectionList<TSubEntity>(string collectionAlias, Action<ISubCollectionListConfig<TSubEntity>>? configure)
        {
            return AddSubCollectionList(collectionAlias, configure);
        }

        IRelatedCollectionListConfig<TEntity, TRelatedEntity> IEditorPaneConfig<TEntity>.AddRelatedCollectionList<TRelatedEntity>(string collectionAlias, Action<IRelatedCollectionListConfig<TEntity, TRelatedEntity>>? configure)
        {
            return AddRelatedCollectionList(collectionAlias, configure);
        }

        IEditorPaneConfig<TEntity> IEditorPaneConfig<TEntity>.SetLabel(string label)
        {
            return SetLabel(label);
        }

        IEditorPaneConfig<TEntity> IEditorPaneConfig<TEntity>.VisibleWhen(Func<TEntity, EntityState, bool> predicate)
        {
            return VisibleWhen(predicate);
        }

    }
}

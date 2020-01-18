using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.Config
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
        internal List<CollectionListConfig> SubCollectionLists { get; set; } = new List<CollectionListConfig>();
        internal List<CollectionListConfig> RelatedCollectionLists { get; set; } = new List<CollectionListConfig>();
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

        private PaneConfig<TEntity> AddSubCollectionList(string collectionAlias)
        {
            var config = new CollectionListConfig(collectionAlias)
            {
                Index = FieldIndex++
            };

            SubCollectionLists.Add(config);

            return this;
        }

        private PaneConfig<TEntity> AddRelatedCollectionList(string collectionAlias)
        {
            var config = new CollectionListConfig(collectionAlias)
            {
                Index = FieldIndex++
            };

            RelatedCollectionLists.Add(config);

            return this;
        }

        private PaneConfig<TEntity> AddSubCollectionList<TSubEntity, TSubRepository>(Action<SubCollectionListConfig<TSubEntity, TSubRepository>>? configure = null)
            where TSubEntity : IEntity
            where TSubRepository : IRepository
        {
            var config = new SubCollectionListConfig<TSubEntity, TSubRepository>(Guid.NewGuid().ToString());

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            SubCollectionLists.Add(config);

            return this;
        }

        private PaneConfig<TEntity> AddRelatedCollectionList<TRelatedEntity, TRelatedRepository>(Action<RelatedCollectionListConfig<TRelatedEntity, TRelatedRepository>>? configure = null)
            where TRelatedEntity : IEntity
            where TRelatedRepository : IRepository
        {
            var config = new RelatedCollectionListConfig<TRelatedEntity, TRelatedRepository>(Guid.NewGuid().ToString());

            configure?.Invoke(config);

            config.Index = FieldIndex++;

            RelatedCollectionLists.Add(config);

            return this;
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

        IDisplayPaneConfig<TEntity> IDisplayPaneConfig<TEntity>.AddSubCollectionList(string collectionAlias)
        {
            return AddSubCollectionList(collectionAlias);
        }

        IDisplayPaneConfig<TEntity> IDisplayPaneConfig<TEntity>.AddSubCollectionList<TSubEntity, TSubRepository>(Action<ISubCollectionListViewConfig<TSubEntity, TSubRepository>>? configure)
        {
            return AddSubCollectionList(configure);
        }

        IDisplayPaneConfig<TEntity> IDisplayPaneConfig<TEntity>.AddRelatedCollectionList(string collectionAlias)
        {
            return AddRelatedCollectionList(collectionAlias);
        }

        IDisplayPaneConfig<TEntity> IDisplayPaneConfig<TEntity>.AddRelatedCollectionList<TRelatedEntity, TRelatedRepository>(Action<IRelatedCollectionListViewConfig<TRelatedEntity, TRelatedRepository>>? configure)
        {
            return AddRelatedCollectionList(configure);
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

        IEditorPaneConfig<TEntity> IEditorPaneConfig<TEntity>.AddSubCollectionList(string collectionAlias)
        {
            return AddSubCollectionList(collectionAlias);
        }

        IEditorPaneConfig<TEntity> IEditorPaneConfig<TEntity>.AddSubCollectionList<TSubEntity, TSubRepository>(Action<ISubCollectionListEditorConfig<TSubEntity, TSubRepository>>? configure)
        {
            return AddSubCollectionList(configure);
        }

        IEditorPaneConfig<TEntity> IEditorPaneConfig<TEntity>.AddRelatedCollectionList(string collectionAlias)
        {
            return AddRelatedCollectionList(collectionAlias);
        }

        IEditorPaneConfig<TEntity> IEditorPaneConfig<TEntity>.AddRelatedCollectionList<TRelatedEntity, TRelatedRepository>(Action<IRelatedCollectionListEditorConfig<TRelatedEntity, TRelatedRepository>>? configure)
        {
            return AddRelatedCollectionList(configure);
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

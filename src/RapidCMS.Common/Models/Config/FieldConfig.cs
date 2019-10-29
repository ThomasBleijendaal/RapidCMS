using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.Config
{
    internal class FieldConfig
    {
        internal int Index { get; set; }

        internal string? Name { get; set; }
        internal string? Description { get; set; }

        internal bool Readonly { get; set; }
        internal Func<object, EntityState, bool> IsVisible { get; set; } = (x, y) => true;
        internal Func<object, EntityState, bool> IsDisabled { get; set; } = (x, y) => false;

        internal IExpressionMetadata? Expression { get; set; }
        internal IPropertyMetadata? Property { get; set; }

        internal RelationConfig? Relation { get; set; }

        internal EditorType Type { get; set; }
        internal Type? CustomType { get; set; }
    }

    internal class FieldConfig<TEntity, TValue> : FieldConfig, IDisplayFieldConfig<TEntity, TValue>, IEditorFieldConfig<TEntity, TValue>
        where TEntity : IEntity
    {
        private FieldConfig<TEntity, TValue> SetName(string name)
        {
            Name = name;
            return this;
        }
        private FieldConfig<TEntity, TValue> SetDescription(string description)
        {
            Description = description;
            return this;
        }
        private FieldConfig<TEntity, TValue> SetType(EditorType type)
        {
            if (type == EditorType.Readonly)
            {
                Readonly = true;
            }

            Type = type;
            return this;
        }
        private FieldConfig<TEntity, TValue> SetType(Type type)
        {
            Type = EditorType.Custom;
            CustomType = type;
            return this;
        }

        private FieldConfig<TEntity, TValue> SetReadonly(bool @readonly = true)
        {
            Readonly = @readonly;

            return this;
        }

        private FieldConfig<TEntity, TValue> SetDataCollection<TDataCollection>()
            where TDataCollection : IDataCollection
        {
            if (Type != EditorType.Custom && Type.GetCustomAttribute<RelationAttribute>()?.Type != RelationType.One)
            {
                throw new InvalidOperationException("Cannot add DataRelation to Editor with no support for RelationType.One");
            }

            var config = new DataProviderRelationConfig(typeof(TDataCollection));

            Relation = config;

            return this;
        }

        private FieldConfig<TEntity, TValue> SetCollectionRelation<TRelatedEntity>(
            string collectionAlias, Action<ICollectionRelationConfig<TEntity, TRelatedEntity>> configure)
        {
            if (Type != EditorType.Custom && !(Type.GetCustomAttribute<RelationAttribute>()?.Type.In(RelationType.One, RelationType.Many) ?? false))
            {
                throw new InvalidOperationException("Cannot add CollectionRelation to Editor with no support for RelationType.One / RelationType.Many");
            }

            var config = new CollectionRelationConfig<TEntity, TRelatedEntity>();

            configure.Invoke(config);

            config.CollectionAlias = collectionAlias;

            Relation = config;

            return this;
        }

        private FieldConfig<TEntity, TValue> SetCollectionRelation<TRelatedEntity, TKey>(
            Expression<Func<TValue, IEnumerable<TKey>>> relatedElements,
            string collectionAlias,
            Action<ICollectionRelationConfig<TEntity, TRelatedEntity>> configure)
        {
            if (Type != EditorType.Custom && !(Type.GetCustomAttribute<RelationAttribute>()?.Type.In(RelationType.Many) ?? false))
            {
                throw new InvalidOperationException("Cannot add CollectionRelation with relatedElements to Editor with no support for RelationType.Many");
            }

            var relatedElementsGetter = PropertyMetadataHelper.GetPropertyMetadata(relatedElements);
            var config = new CollectionRelationConfig<TEntity, TRelatedEntity>(relatedElementsGetter ?? throw new InvalidExpressionException(nameof(relatedElements)));

            configure.Invoke(config);

            config.CollectionAlias = collectionAlias;

            Relation = config;

            return this;
        }

        private FieldConfig<TEntity, TValue> VisibleWhen(Func<TEntity, EntityState, bool>? predicate = null)
        {
            IsVisible = (entity, state) => predicate.Invoke((TEntity)entity, state);

            return this;
        }

        private FieldConfig<TEntity, TValue> DisableWhen(Func<TEntity, EntityState, bool> predicate)
        {
            IsDisabled = (entity, state) => predicate.Invoke((TEntity)entity, state);

            return this;
        }

        IDisplayFieldConfig<TEntity, TValue> IDisplayFieldConfig<TEntity, TValue>.SetName(string name)
        {
            SetName(name);
            return this;
        }

        IDisplayFieldConfig<TEntity, TValue> IDisplayFieldConfig<TEntity, TValue>.SetDescription(string description)
        {
            SetDescription(description);
            return this;
        }

        IDisplayFieldConfig<TEntity, TValue> IDisplayFieldConfig<TEntity, TValue>.VisibleWhen(Func<TEntity, EntityState, bool> predicate)
        {
            VisibleWhen(predicate);
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.SetName(string name)
        {
            SetName(name);
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.SetDescription(string description)
        {
            SetDescription(description);
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.SetType(EditorType type)
        {
            SetType(type);
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.SetType(Type type)
        {
            SetType(type);
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.SetReadonly(bool @readonly)
        {
            SetReadonly(@readonly);
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.SetDataCollection<TDataCollection>()
        {
            SetDataCollection<TDataCollection>();
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.SetCollectionRelation<TRelatedEntity>(
            string collectionAlias, Action<ICollectionRelationConfig<TEntity, TRelatedEntity>> configure)
        {
            SetCollectionRelation(collectionAlias, configure);
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.SetCollectionRelation<TRelatedEntity, TKey>(
            Expression<Func<TValue, IEnumerable<TKey>>> relatedElements, string collectionAlias, Action<ICollectionRelationConfig<TEntity, TRelatedEntity>> configure)
        {
            SetCollectionRelation(relatedElements, collectionAlias, configure);
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.VisibleWhen(Func<TEntity, EntityState, bool> predicate)
        {
            VisibleWhen(predicate);
            return this;
        }

        IEditorFieldConfig<TEntity, TValue> IEditorFieldConfig<TEntity, TValue>.DisableWhen(Func<TEntity, EntityState, bool> predicate)
        {
            DisableWhen(predicate);
            return this;
        }
    }
}

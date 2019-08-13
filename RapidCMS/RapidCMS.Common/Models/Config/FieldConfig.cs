using System;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.Config
{
    public class FieldConfig
    {
        internal int Index { get; set; }

        internal string Name { get; set; }
        internal string Description { get; set; }

        internal bool Readonly { get; set; }
        internal Func<object, bool> IsVisible { get; set; } = (x) => true;

        internal IExpressionMetadata? Expression { get; set; }
        internal IPropertyMetadata? Property { get; set; }

        internal RelationConfig? Relation { get; set; }

        internal EditorType Type { get; set; }
        internal Type CustomType { get; set; }
    }

    public interface IFieldConfig<TEntity>
        where TEntity : IEntity
    {

    }

    public interface IReadonlyFieldConfig<TEntity> : IFieldConfig<TEntity>
        where TEntity : IEntity
    {
        IReadonlyFieldConfig<TEntity> SetName(string name);
        IReadonlyFieldConfig<TEntity> SetDescription(string description);
        IReadonlyFieldConfig<TEntity> VisibleWhen(Func<TEntity, bool> predicate);
    }

    public interface IFullFieldConfig<TEntity> : IFieldConfig<TEntity>
        where TEntity : IEntity
    {
        IFullFieldConfig<TEntity> SetName(string name);
        IFullFieldConfig<TEntity> SetDescription(string description);
        IFullFieldConfig<TEntity> SetType(EditorType type);
        IFullFieldConfig<TEntity> SetType(Type type);
        IFullFieldConfig<TEntity> SetReadonly(bool @readonly = true);
        IFullFieldConfig<TEntity> SetDataCollection<TDataCollection>()
            where TDataCollection : IDataCollection;
        IFullFieldConfig<TEntity> SetCollectionRelation<TRelatedEntity>(string collectionAlias, Action<CollectionRelationConfig<TEntity, TRelatedEntity>> configure);
        IFullFieldConfig<TEntity> VisibleWhen(Func<TEntity, bool> predicate);
    }

    public class FieldConfig<TEntity> : FieldConfig, IFieldConfig<TEntity>, IReadonlyFieldConfig<TEntity>, IFullFieldConfig<TEntity>
        where TEntity : IEntity
    {
        public FieldConfig<TEntity> SetName(string name)
        {
            Name = name;
            return this;
        }
        public FieldConfig<TEntity> SetDescription(string description)
        {
            Description = description;
            return this;
        }
        public FieldConfig<TEntity> SetType(EditorType type)
        {
            if (type == EditorType.Readonly)
            {
                Readonly = true;
            }

            Type = type;
            return this;
        }
        public FieldConfig<TEntity> SetType(Type type)
        {
            Type = EditorType.Custom;
            CustomType = type;
            return this;
        }

        public FieldConfig<TEntity> SetReadonly(bool @readonly = true)
        {
            Readonly = @readonly;

            return this;
        }

        public FieldConfig<TEntity> SetDataCollection<TDataCollection>()
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

        // TODO: perhaps add alias to differentiate between duplicate relations
        public FieldConfig<TEntity> SetCollectionRelation<TRelatedEntity>(string collectionAlias, Action<CollectionRelationConfig<TEntity, TRelatedEntity>> configure)
        {
            if (Type != EditorType.Custom && !(Type.GetCustomAttribute<RelationAttribute>()?.Type.In(RelationType.One, RelationType.Many) ?? false))
            {
                throw new InvalidOperationException("Cannot add CollectionRelation to Editor with no support for RelationType.Many");
            }

            var config = new CollectionRelationConfig<TEntity, TRelatedEntity>();

            configure.Invoke(config);

            config.CollectionAlias = collectionAlias;

            Relation = config;

            return this;
        }

        public FieldConfig<TEntity> VisibleWhen(Func<TEntity, bool> predicate)
        {
            IsVisible = (entity) => predicate.Invoke((TEntity)entity);

            return this;
        }

        IReadonlyFieldConfig<TEntity> IReadonlyFieldConfig<TEntity>.SetName(string name)
        {
            SetName(name);
            return this;
        }

        IReadonlyFieldConfig<TEntity> IReadonlyFieldConfig<TEntity>.SetDescription(string description)
        {
            SetDescription(description);
            return this;
        }

        IReadonlyFieldConfig<TEntity> IReadonlyFieldConfig<TEntity>.VisibleWhen(Func<TEntity, bool> predicate)
        {
            VisibleWhen(predicate);
            return this;
        }

        IFullFieldConfig<TEntity> IFullFieldConfig<TEntity>.SetName(string name)
        {
            SetName(name);
            return this;
        }

        IFullFieldConfig<TEntity> IFullFieldConfig<TEntity>.SetDescription(string description)
        {
            SetDescription(description);
            return this;
        }

        IFullFieldConfig<TEntity> IFullFieldConfig<TEntity>.SetType(EditorType type)
        {
            SetType(type);
            return this;
        }

        IFullFieldConfig<TEntity> IFullFieldConfig<TEntity>.SetType(Type type)
        {
            SetType(type);
            return this;
        }

        IFullFieldConfig<TEntity> IFullFieldConfig<TEntity>.SetReadonly(bool @readonly)
        {
            SetReadonly(@readonly);
            return this;
        }

        IFullFieldConfig<TEntity> IFullFieldConfig<TEntity>.SetDataCollection<TDataCollection>()
        {
            SetDataCollection<TDataCollection>();
            return this;
        }

        IFullFieldConfig<TEntity> IFullFieldConfig<TEntity>.SetCollectionRelation<TRelatedEntity>(string collectionAlias, Action<CollectionRelationConfig<TEntity, TRelatedEntity>> configure)
        {
            SetCollectionRelation(collectionAlias, configure);
            return this;
        }

        IFullFieldConfig<TEntity> IFullFieldConfig<TEntity>.VisibleWhen(Func<TEntity, bool> predicate)
        {
            VisibleWhen(predicate);
            return this;
        }
    }
}

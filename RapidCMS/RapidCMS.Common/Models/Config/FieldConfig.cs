using System;
using System.Linq.Expressions;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.ValueMappers;

#nullable enable

namespace RapidCMS.Common.Models.Config
{
    public class FieldConfig
    {
        internal int Index { get; set; }

        internal string Name { get; set; }
        internal string Description { get; set; }

        internal bool Readonly { get; set; }

        internal IPropertyMetadata Property { get; set; }
        internal Type ValueMapperType { get; set; }
        internal RelationConfig? OneToManyRelation { get; set; }

        internal EditorType Type { get; set; }
        internal Type CustomType { get; set; }
    }

    public class FieldConfig<TEntity> : FieldConfig
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
            Type = type;
            return this;
        }
        public FieldConfig<TEntity> SetType(Type type)
        {
            Type = EditorType.Custom;
            CustomType = type;
            return this;
        }

        // TODO: check for mapper compatibility with value type in NodeProperty
        public FieldConfig<TEntity> SetValueMapper<TValueMapper>()
            where TValueMapper : IValueMapper
        {
            ValueMapperType = typeof(TValueMapper);

            return this;
        }

        public FieldConfig<TEntity> SetReadonly(bool @readonly = true)
        {
            Readonly = @readonly;

            return this;
        }

        public FieldConfig<TEntity> SetDataRelation<TDataCollection>()
            where TDataCollection : IDataCollection
        {
            if (Type != EditorType.Custom && Type.GetCustomAttribute<RelationAttribute>().Type != RelationType.One)
            {
                throw new InvalidOperationException("Cannot add DataRelation to Editor with no support for RelationType.One");
            }

            var config = new DataProviderRelationConfig
            {
                DataCollectionType = typeof(TDataCollection)
            };

            OneToManyRelation = config;

            return this;
        }

        // TODO: perhaps add alias to differentiate between duplicate relations
        public FieldConfig<TEntity> SetCollectionRelation<TRelatedEntity>(string collectionAlias, Action<RelationCollectionConfig<TEntity, TRelatedEntity>> configure)
        {
            if (Type != EditorType.Custom && Type.GetCustomAttribute<RelationAttribute>().Type != RelationType.Many)
            {
                throw new InvalidOperationException("Cannot add CollectionRelation to Editor with no support for RelationType.Many");
            }

            var config = new RelationCollectionConfig<TEntity, TRelatedEntity>();

            configure.Invoke(config);

            config.CollectionAlias = collectionAlias;

            OneToManyRelation = config;

            return this;
        }
    }

    public class RelationConfig
    {
    }

    public class DataProviderRelationConfig : RelationConfig
    {
        internal Type DataCollectionType { get; set; }
    }

    public class CollectionRelationConfig : RelationConfig
    {
        internal string CollectionAlias { get; set; }
        internal Type RelatedEntityType { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal IExpressionMetadata DisplayProperty { get; set; }
    }

    public class RelationCollectionConfig<TEntity, TRelatedEntity> : CollectionRelationConfig
    {
        public RelationCollectionConfig()
        {
            RelatedEntityType = typeof(TRelatedEntity);
        }

        public RelationCollectionConfig<TEntity, TRelatedEntity> SetIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
        {
            IdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

            return this;
        }

        public RelationCollectionConfig<TEntity, TRelatedEntity> SetDisplayProperty(Expression<Func<TRelatedEntity, string>> propertyExpression)
        {
            DisplayProperty = PropertyMetadataHelper.GetExpressionMetadata(propertyExpression);

            return this;
        }
    }
}

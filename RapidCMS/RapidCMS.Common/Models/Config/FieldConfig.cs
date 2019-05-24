using System;
using System.Linq.Expressions;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Helpers;
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
        internal OneToManyRelationConfig? OneToManyRelation { get; set; }

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

        // TODO: move all config into this function? (no SetId + SetDisplay?)
        // TODO: rename to select source or something
        public FieldConfig<TEntity> SetOneToManyRelation<TDataCollection>()
            where TDataCollection : IDataCollection
        {
            var config = new OneToManyRelationDataProviderConfig
            {
                DataCollectionType = typeof(TDataCollection)
            };

            OneToManyRelation = config;

            return this;
        }

        public FieldConfig<TEntity> SetOneToManyRelation<TRelatedEntity>(string collectionAlias, Action<OneToManyRelationCollectionConfig<TEntity, TRelatedEntity>> configure)
        {
            var config = new OneToManyRelationCollectionConfig<TEntity, TRelatedEntity>();

            configure.Invoke(config);

            config.CollectionAlias = collectionAlias;

            OneToManyRelation = config;

            return this;
        }
    }

    public class OneToManyRelationConfig
    {
    }

    public class OneToManyRelationDataProviderConfig : OneToManyRelationConfig
    {
        internal Type DataCollectionType { get; set; }
    }

    public class OneToManyRelationCollectionConfig : OneToManyRelationConfig
    {
        internal string CollectionAlias { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal IExpressionMetadata DisplayProperty { get; set; }
    }

    public class OneToManyRelationCollectionConfig<TEntity, TRelatedEntity> : OneToManyRelationCollectionConfig
    {
        public OneToManyRelationCollectionConfig<TEntity, TRelatedEntity> SetIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
        {
            IdProperty = PropertyMetadataHelper.GetPropertyMetadata(propertyExpression);

            return this;
        }

        public OneToManyRelationCollectionConfig<TEntity, TRelatedEntity> SetDisplayProperty(Expression<Func<TRelatedEntity, string>> propertyExpression)
        {
            DisplayProperty = PropertyMetadataHelper.GetExpressionMetadata(propertyExpression);

            return this;
        }
    }
}

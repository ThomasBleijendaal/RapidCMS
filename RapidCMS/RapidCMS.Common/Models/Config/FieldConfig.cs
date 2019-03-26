using System;
using System.Linq.Expressions;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Interfaces;

#nullable enable

namespace RapidCMS.Common.Models.Config
{
    public class FieldConfig
    {
        internal int Index { get; set; }

        internal string Name { get; set; }
        internal string Description { get; set; }

        internal bool Readonly { get; set; }

        internal PropertyMetadata NodeProperty { get; set; }
        internal IValueMapper ValueMapper { get; set; }
        internal Type ValueMapperType { get; set; }
        internal OneToManyRelationConfig? OneToManyRelation { get; set; }

        internal EditorType Type { get; set; }
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

        // TODO: check for mapper compatibility with value type in NodeProperty
        public FieldConfig<TEntity> SetValueMapper<TValue>(ValueMapper<TValue> valueMapper)
        {
            ValueMapper = valueMapper;

            return this;
        }

        // TODO: check for mapper compatibility with value type in NodeProperty
        public FieldConfig<TEntity> SetValueMapper<TValueMapper>()
            where TValueMapper : IValueMapper
        {
            ValueMapperType = typeof(IValueMapper);

            return this;
        }

        public FieldConfig<TEntity> SetReadonly(bool @readonly = true)
        {
            Readonly = @readonly;

            return this;
        }

        // TODO: move all config into this function? (no SetId + SetDisplay?)
        // TODO: rename to select source or something
        public FieldConfig<TEntity> SetOneToManyRelation<TDataProvider>()
            where TDataProvider : IDataProvider
        {
            var config = new OneToManyRelationDataProviderConfig
            {
                DataProviderType = typeof(TDataProvider)
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
        internal Type DataProviderType { get; set; }
    }

    public class OneToManyRelationCollectionConfig : OneToManyRelationConfig
    {
        internal string CollectionAlias { get; set; }
        internal PropertyMetadata IdProperty { get; set; }
        internal PropertyMetadata DisplayProperty { get; set; }
    }

    public class OneToManyRelationCollectionConfig<TEntity, TRelatedEntity> : OneToManyRelationCollectionConfig
    {
        public OneToManyRelationCollectionConfig<TEntity, TRelatedEntity> SetIdProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
        {
            IdProperty = PropertyMetadataHelper.Create(propertyExpression);

            return this;
        }
        public OneToManyRelationCollectionConfig<TEntity, TRelatedEntity> SetDisplayProperty<TValue>(Expression<Func<TRelatedEntity, TValue>> propertyExpression)
        {
            DisplayProperty = PropertyMetadataHelper.Create(propertyExpression);

            return this;
        }
    }
}

using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.ValueMappers;

namespace RapidCMS.Common.Models.Config
{
    public class PropertyConfig
    {
        internal string Name { get; set; }
        internal string Description { get; set; }

        internal PropertyMetadata NodeProperty { get; set; }

        internal IValueMapper ValueMapper { get; set; }
        internal Type ValueMapperType { get; set; }
    }

    public class PropertyConfig<TEntity> : PropertyConfig
        where TEntity : IEntity
    {
        
        public PropertyConfig<TEntity> SetName(string name)
        {
            Name = name;
            return this;
        }
        public PropertyConfig<TEntity> SetDescription(string description)
        {
            Description = description;
            return this;
        }

        // TODO: check for mapper compatibility with value type in NodeProperty
        public PropertyConfig<TEntity> SetValueMapper<TValue>(ValueMapper<TValue> valueMapper)
        {
            ValueMapper = valueMapper;

            return this;
        }

        // TODO: check for mapper compatibility with value type in NodeProperty
        public PropertyConfig<TEntity> SetValueMapper<TValueMapper>()
            where TValueMapper : IValueMapper
        {
            ValueMapperType = typeof(IValueMapper);

            return this;
        }
    }
}

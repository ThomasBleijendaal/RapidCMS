using System;
using System.Collections.Generic;
using System.Text;
using RapidCMS.Common.Interfaces;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    public static class FieldConfigExtensions
    {
        public static Field ToField(this FieldConfig field)
        {
            return new Field
            {
                DataType = field.Type,
                Description = field.Description,
                Name = field.Name,
                NodeProperty = field.NodeProperty,
                Readonly = field.Readonly,
                ValueMapper = field.ValueMapper ?? new DefaultValueMapper(),
                ValueMapperType = field.ValueMapperType,

                OneToManyRelation = field.OneToManyRelation?.ToOneToManyRelation()
            };
        }
    }

    public static class RelationConfigExtensions
    {
        public static OneToManyRelation ToOneToManyRelation(this OneToManyRelationConfig config)
        {
            return new OneToManyRelation
            {
                CollectionAlias = config.CollectionAlias,
                DisplayProperty = config.DisplayProperty,
                IdProperty = config.IdProperty
            };
        }
    }
}

using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.ValueMappers;

namespace RapidCMS.Common.Extensions
{
    internal static class FieldConfigExtensions
    {
        public static Field ToField(this FieldConfig field)
        {
            if (field.Type == EditorType.Custom)
            {
                return new CustomField(field.CustomType)
                {
                    Index = field.Index,

                    DataType = field.Type,
                    Description = field.Description,
                    Name = field.Name,
                    Property = field.Property,
                    Readonly = field.Readonly,
                    ValueMapperType = field.ValueMapperType ?? DefaultValueMapper.GetDefaultValueMapper(field.Property.PropertyType),

                    Relation = field.Relation?.ToRelation()
                };
            }
            else
            {
                return new PropertyField
                {
                    Index = field.Index,

                    DataType = field.Type,
                    Description = field.Description,
                    Name = field.Name,
                    Property = field.Property,
                    Readonly = field.Readonly,
                    ValueMapperType = field.ValueMapperType ?? DefaultValueMapper.GetDefaultValueMapper(field.Property.PropertyType),

                    Relation = field.Relation?.ToRelation()
                };
            }
        }
    }

    internal static class PropertyConfigExtensions
    {
        public static Field ToField(this PropertyConfig property)
        {
            return new ExpressionField
            {
                Index = property.Index,

                Description = property.Description,
                Name = property.Name,
                Expression = property.Property,

                Readonly = true
            };
        }
    }

    internal static class SubCollectionListConfigExtensions
    {
        public static SubCollectionList ToSubCollectionList(this SubCollectionListConfig subCollection)
        {
            return new SubCollectionList
            {
                Index = subCollection.Index,
                CollectionAlias = subCollection.CollectionAlias
            };
        }
    }
}

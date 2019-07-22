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
                    IsVisible = field.IsVisible,
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
                    IsVisible = field.IsVisible,
                    ValueMapperType = field.ValueMapperType ?? DefaultValueMapper.GetDefaultValueMapper(field.Property.PropertyType),

                    Relation = field.Relation?.ToRelation()
                };
            }
        }
    }
}

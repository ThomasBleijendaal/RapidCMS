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
                    NodeProperty = field.NodeProperty,
                    Readonly = field.Readonly,
                    ValueMapperType = field.ValueMapperType ?? typeof(DefaultValueMapper),

                    OneToManyRelation = field.OneToManyRelation?.ToOneToManyRelation()
                };
            }
            else
            {
                return new Field
                {
                    Index = field.Index,

                    DataType = field.Type,
                    Description = field.Description,
                    Name = field.Name,
                    NodeProperty = field.NodeProperty,
                    Readonly = field.Readonly,
                    ValueMapperType = field.ValueMapperType ?? typeof(DefaultValueMapper),

                    OneToManyRelation = field.OneToManyRelation?.ToOneToManyRelation()
                };
            }
        }
    }

    public static class RelationConfigExtensions
    {
        public static OneToManyRelation ToOneToManyRelation(this OneToManyRelationConfig config)
        {
            return config switch
            {
                OneToManyRelationCollectionConfig collectionConfig => new OneToManyCollectionRelation
                {
                    CollectionAlias = collectionConfig.CollectionAlias,
                    DisplayProperty = collectionConfig.DisplayProperty,
                    IdProperty = collectionConfig.IdProperty
                },
                OneToManyRelationDataProviderConfig dataProviderConfig => new OneToManyDataProviderRelation
                {
                    DataProviderType = dataProviderConfig.DataProviderType
                },
                _ => default(OneToManyRelation)
            };
        }
    }
}

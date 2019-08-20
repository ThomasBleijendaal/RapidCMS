using System;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class FieldConfigExtensions
    {
        public static Field ToField(this FieldConfig field)
        {
            if (field.Type == EditorType.Custom)
            {
                return new CustomField(field.Property!, field.CustomType)
                {
                    Index = field.Index,

                    DataType = field.Type,
                    Description = field.Description,
                    Name = field.Name,
                    Readonly = field.Readonly,
                    IsVisible = field.IsVisible,

                    Relation = field.Relation?.ToRelation()
                };
            }
            else
            {
                if (field.Property != null)
                {
                    return new PropertyField(field.Property)
                    {
                        Index = field.Index,

                        DataType = field.Type,
                        Description = field.Description,
                        Name = field.Name,
                        Readonly = field.Readonly,
                        IsVisible = field.IsVisible,

                        Relation = field.Relation?.ToRelation()
                    };
                }
                else if (field.Expression != null)
                {
                    return new ExpressionField(field.Expression)
                    {
                        Index = field.Index,

                        Description = field.Description,
                        Name = field.Name,

                        Readonly = true,
                        IsVisible = field.IsVisible
                    };
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}

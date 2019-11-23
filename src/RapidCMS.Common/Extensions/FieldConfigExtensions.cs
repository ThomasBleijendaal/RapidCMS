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
            if (field.Property != null)
            {
                if (field.EditorType == EditorType.Custom)
                {
                    return new CustomPropertyField(field.Property!, field.CustomType!)
                    {
                        Index = field.Index,

                        EditorType = field.EditorType,
                        Description = field.Description,
                        Name = field.Name,
                        IsVisible = field.IsVisible,
                        IsDisabled = field.IsDisabled,

                        Relation = field.Relation?.ToRelation()
                    };
                }
                else
                {
                    return new PropertyField(field.Property)
                    {
                        Index = field.Index,

                        EditorType = field.EditorType,
                        Description = field.Description,
                        Name = field.Name,
                        IsVisible = field.IsVisible,
                        IsDisabled = field.IsDisabled,

                        Relation = field.Relation?.ToRelation()
                    };
                }
            }
            else if (field.Expression != null)
            {
                if (field.DisplayType == DisplayType.Custom)
                {
                    return new CustomExpressionField(field.Expression, field.CustomType!)
                    {
                        Index = field.Index,
                        
                        DisplayType = field.DisplayType,
                        Description = field.Description,
                        Name = field.Name,
                        IsVisible = field.IsVisible,
                        IsDisabled = field.IsDisabled
                    };
                }
                else
                {
                    return new ExpressionField(field.Expression)
                    {
                        Index = field.Index,

                        DisplayType = field.DisplayType,
                        Description = field.Description,
                        Name = field.Name,
                        IsVisible = field.IsVisible,
                        IsDisabled = field.IsDisabled
                    };
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}

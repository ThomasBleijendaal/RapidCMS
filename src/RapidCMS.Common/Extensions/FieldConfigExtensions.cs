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
            if (field.EditorType == EditorType.Custom && field.Property != null)
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
            else if (field.EditorType != EditorType.None && field.Property != null)
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
            else if (field.DisplayType != DisplayType.None && field.Property != null)
            {
                return new ExpressionField(field.Property)
                {
                    Index = field.Index,

                    DisplayType = field.DisplayType,
                    Description = field.Description,
                    Name = field.Name,
                    IsVisible = field.IsVisible,
                    IsDisabled = field.IsDisabled
                };
            }
            else if (field.DisplayType == DisplayType.Custom && field.Expression != null)
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
            else if (field.DisplayType != DisplayType.None && field.Expression != null)
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
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}

using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class FieldSetup : IFieldSetup
    {
        internal FieldSetup(FieldConfig field)
        {
            Index = field.Index;
            Description = field.Description;
            Details = field.Details;
            Name = field.Name;
            Placeholder = field.Placeholder;
            Property = field.Property;
            Expression = field.Expression;
            OrderByExpression = field.OrderByExpression;
            DefaultOrder = field.DefaultOrder;
            IsVisible = field.IsVisible;
            IsDisabled = field.IsDisabled;
        }

        public int Index { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public MarkupString? Details { get; set; }
        public string? Placeholder { get; set; }

        public IPropertyMetadata? Property { get; set; }
        public IExpressionMetadata? Expression { get; set; }
        public IPropertyMetadata? OrderByExpression { get; set; }
        public OrderByType DefaultOrder { get; set; }

        public Func<object, EntityState, bool> IsVisible { get; set; }
        public Func<object, EntityState, bool> IsDisabled { get; set; }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class FieldUI : ElementUI
    {
        internal FieldUI(FieldSetup field) : base(field.IsVisible, field.IsDisabled)
        {
            Index = field.Index;
            Description = field.Description;
            Details = field.Details;
            Name = field.Name;
            Placeholder = field.Placeholder;
            Expression = field.Expression;
            Property = field.Property;
            OrderByExpression = field.OrderByExpression;
            SortDirection = field.DefaultOrder;
            Configuration = field.Configuration;
        }

        public int Index { get; private set; }
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public MarkupString? Details { get; private set; }
        public string? Placeholder { get; private set; }

        public bool IsSortable() => OrderByExpression != null;

        public IPropertyMetadata? Property { get; internal set; }
        public IExpressionMetadata? Expression { get; internal set; }
        internal IPropertyMetadata? OrderByExpression { get; private set; }
        public OrderByType SortDirection { get; private set; }

        public Func<object, EntityState, Task<object?>>? Configuration { get; set; }
    }
}

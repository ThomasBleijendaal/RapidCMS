using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IFieldSetup
    {
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

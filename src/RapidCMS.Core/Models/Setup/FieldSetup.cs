using System;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class FieldSetup
    {
        internal FieldSetup(FieldConfig field)
        {
            Index = field.Index;
            Description = field.Description;
            Name = field.Name;
            OrderByExpression = field.OrderByExpression;
            DefaultOrder = field.DefaultOrder;
            IsVisible = field.IsVisible;
            IsDisabled = field.IsDisabled;
        }

        internal int Index { get; set; }
        
        internal string? Name { get; set; }
        internal string? Description { get; set; }

        internal IPropertyMetadata? OrderByExpression { get; set; }
        internal OrderByType DefaultOrder { get; set; }

        internal Func<object, EntityState, bool> IsVisible { get; set; }
        internal Func<object, EntityState, bool> IsDisabled { get; set; }
    }
}

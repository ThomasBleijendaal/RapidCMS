using System;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Models.Setup
{
    internal class FieldSetup
    {
        internal int Index { get; set; }
        
        internal string? Name { get; set; }
        internal string? Description { get; set; }

        internal IPropertyMetadata? OrderByExpression { get; set; }
        internal OrderByType DefaultOrder { get; set; }

        internal Func<object, EntityState, bool> IsVisible { get; set; }
        internal Func<object, EntityState, bool> IsDisabled { get; set; }
    }
}

using System;
using System.Linq.Expressions;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class Field
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

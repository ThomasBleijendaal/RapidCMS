using System;
using RapidCMS.Common.Enums;


namespace RapidCMS.Common.Models
{
    internal class Field
    {
        internal int Index { get; set; }
        
        internal string Name { get; set; }
        internal string Description { get; set; }

        internal bool Readonly { get; set; } = true;
        internal Func<object, EntityState, bool> IsVisible { get; set; }
        internal Func<object, EntityState, bool> IsDisabled { get; set; }

        internal EditorType DataType { get; set; } = EditorType.Readonly;
    }
}

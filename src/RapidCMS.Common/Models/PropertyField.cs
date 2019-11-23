using System;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class PropertyField : Field
    {
        public PropertyField(IPropertyMetadata property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }
        
        internal EditorType EditorType { get; set; } = EditorType.Readonly;
        internal IPropertyMetadata Property { get; set; }

        internal Relation? Relation { get; set; }
    }
}

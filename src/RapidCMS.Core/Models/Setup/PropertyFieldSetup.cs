using System;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Models.Setup
{
    internal class PropertyFieldSetup : FieldSetup
    {
        public PropertyFieldSetup(IPropertyMetadata property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }
        
        internal EditorType EditorType { get; set; } = EditorType.Readonly;
        internal IPropertyMetadata Property { get; set; }

        internal RelationSetup? Relation { get; set; }
    }
}

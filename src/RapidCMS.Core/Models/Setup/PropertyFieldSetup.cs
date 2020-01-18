using System;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class PropertyFieldSetup : FieldSetup
    {
        internal PropertyFieldSetup(FieldConfig field) : base(field)
        {
            Property = field.Property ?? throw new ArgumentNullException(nameof(field.Property));
            EditorType = field.EditorType;

            if (field.Relation != null)
            {
                Relation = ConfigProcessingHelper.ProcessRelation(field.Relation);
            }
        }
        
        internal EditorType EditorType { get; set; } = EditorType.Readonly;
        internal IPropertyMetadata Property { get; set; }

        internal RelationSetup? Relation { get; set; }
    }
}

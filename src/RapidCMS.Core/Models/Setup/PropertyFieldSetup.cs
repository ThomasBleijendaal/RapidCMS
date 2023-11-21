using System;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup;

public class PropertyFieldSetup : FieldSetup
{
    internal PropertyFieldSetup(FieldConfig field) : base(field)
    {
        Property = field.Property ?? throw new ArgumentNullException(nameof(field.Property));
        EditorType = field.EditorType;
    }

    public PropertyFieldSetup() : base(default)
    {

    }

    public EditorType EditorType { get; set; } = EditorType.Readonly;

    public RelationSetup? Relation { get; set; }
}

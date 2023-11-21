using System;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup;

public class CustomPropertyFieldSetup : PropertyFieldSetup
{
    internal CustomPropertyFieldSetup(FieldConfig field, Type customFieldType) : base(field)
    {
        CustomType = customFieldType ?? throw new ArgumentNullException(nameof(customFieldType));
    }

    public CustomPropertyFieldSetup(Type customFieldType) : base()
    {
        CustomType = customFieldType ?? throw new ArgumentNullException(nameof(customFieldType));
    }

    public Type CustomType { get; set; }
}

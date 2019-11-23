using System;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class CustomPropertyField : PropertyField
    {
        public CustomPropertyField(IPropertyMetadata property, Type customFieldType) : base(property)
        {
            CustomType = customFieldType ?? throw new ArgumentNullException(nameof(customFieldType));
        }

        internal Type CustomType { get; set; }
    }
}

using System;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class CustomField : PropertyField
    {
        public CustomField(IPropertyMetadata property, Type customFieldType) : base(property)
        {
            Alias = customFieldType?.FullName ?? throw new ArgumentNullException(nameof(customFieldType));
        }

        internal string Alias { get; set; }
    }
}

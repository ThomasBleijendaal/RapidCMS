using System;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Models.Setup
{
    internal class CustomPropertyFieldSetup : PropertyFieldSetup
    {
        public CustomPropertyFieldSetup(IPropertyMetadata property, Type customFieldType) : base(property)
        {
            CustomType = customFieldType ?? throw new ArgumentNullException(nameof(customFieldType));
        }

        internal Type CustomType { get; set; }
    }
}

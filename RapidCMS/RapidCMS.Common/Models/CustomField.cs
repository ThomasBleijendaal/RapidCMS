using System;


namespace RapidCMS.Common.Models
{
    internal class CustomField : PropertyField
    {
        internal CustomField(Type customFieldType)
        {
            Alias = customFieldType?.FullName ?? throw new ArgumentNullException(nameof(customFieldType));
        }

        internal string Alias { get; set; }
    }
}

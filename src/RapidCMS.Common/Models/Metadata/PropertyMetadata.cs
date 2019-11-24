using System;

namespace RapidCMS.Common.Models.Metadata
{
    internal class PropertyMetadata : IPropertyMetadata
    {
        public PropertyMetadata(Type propertyType, string propertyName, Func<object, object> getter, Type objectType, string fingerprint)
        {
            PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            Getter = getter ?? throw new ArgumentNullException(nameof(getter));
            ObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
            Fingerprint = fingerprint ?? throw new ArgumentNullException(nameof(fingerprint));
        }

        public Type PropertyType { get; private set; }
        public string PropertyName { get; private set; }
        public Func<object, object> Getter { get; private set; }
        public Type ObjectType { get; private set; }

        public string Fingerprint { get; private set; }
    }
}

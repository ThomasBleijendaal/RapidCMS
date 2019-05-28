using System;

namespace RapidCMS.Common.Models.Metadata
{
    internal class PropertyMetadata : IPropertyMetadata
    {
        public Type PropertyType { get; internal set; }
        public string PropertyName { get; internal set; }
        public Func<object, object> Getter { get; internal set; }
        public Type ObjectType { get; internal set; }

        public string Fingerprint { get; internal set; }
    }
}

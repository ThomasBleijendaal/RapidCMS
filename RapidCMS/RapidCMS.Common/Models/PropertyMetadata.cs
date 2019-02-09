using System;

namespace RapidCMS.Common.Models
{
    internal class PropertyMetadata
    {
        internal Type ObjectType { get; set; }
        internal string PropertyName { get; set; }
        internal Type PropertyType { get; set; }
        internal Func<object, object> Getter { get; set; }
        internal Action<object, object> Setter { get; set; }
    }
}

using System;

namespace RapidCMS.Common.Models
{
    // TODO: add interface
    public class PropertyMetadata
    {
        internal Type ObjectType { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        internal Func<object, object> Getter { get; set; }
        internal Action<object, object> Setter { get; set; }
    }
}

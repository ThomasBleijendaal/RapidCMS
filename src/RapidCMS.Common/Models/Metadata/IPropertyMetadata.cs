using System;

namespace RapidCMS.Common.Models.Metadata
{
    public interface IPropertyMetadata
    {
        Type PropertyType { get; }
        string PropertyName { get; }
        Type ObjectType { get; }
        Func<object, object> Getter { get; }

        string Fingerprint { get; }
    }
}

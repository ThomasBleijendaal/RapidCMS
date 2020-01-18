using System;

namespace RapidCMS.Core.Abstractions.Metadata
{
    public interface IFullPropertyMetadata : IPropertyMetadata
    {
        Action<object, object> Setter { get; }
    }
}

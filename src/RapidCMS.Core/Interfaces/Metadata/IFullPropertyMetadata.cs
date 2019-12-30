using System;

namespace RapidCMS.Core.Interfaces.Metadata
{
    public interface IFullPropertyMetadata : IPropertyMetadata
    {
        Action<object, object> Setter { get; }
    }
}

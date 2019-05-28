using System;

namespace RapidCMS.Common.Models.Metadata
{
    public interface IFullPropertyMetadata : IPropertyMetadata
    {
        Action<object, object> Setter { get; }
    }
}

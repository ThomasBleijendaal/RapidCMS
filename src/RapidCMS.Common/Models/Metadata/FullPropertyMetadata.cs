using System;

namespace RapidCMS.Common.Models.Metadata
{
    internal class FullPropertyMetadata : PropertyMetadata, IFullPropertyMetadata
    {
        public Action<object, object> Setter { get; internal set; }
    }
}

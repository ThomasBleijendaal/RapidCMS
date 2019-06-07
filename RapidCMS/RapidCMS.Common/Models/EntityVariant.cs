using System;

#nullable enable

namespace RapidCMS.Common.Models
{
    public class EntityVariant
    {
        public string Name { get; internal set; }
        public string Icon { get; internal set; }
        public Type Type { get; internal set; }
        public string Alias { get; internal set; }
    }
}

using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ShortName { get; set; }

        public string? Placeholder { get; set; }

        public int Index { get; set; }

        public Type? EditorType { get; set; }

        public OrderByType OrderByType { get; set; }
    }
}

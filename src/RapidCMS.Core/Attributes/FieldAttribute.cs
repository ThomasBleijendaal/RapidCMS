using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        /// <summary>
        /// Used as label for Node Editor
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Used as description for Node Editor
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Used as label in List View
        /// </summary>
        public string? ListName { get; set; }

        /// <summary>
        /// Placeholder in editor
        /// </summary>
        public string? Placeholder { get; set; }

        /// <summary>
        /// Order in which fields are displayed
        /// </summary>
        public int Index { get; set; }

        public Type? EditorType { get; set; }

        public OrderByType OrderByType { get; set; }
    }
}

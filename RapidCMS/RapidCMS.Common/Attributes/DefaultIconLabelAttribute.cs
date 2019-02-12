using System;

namespace RapidCMS.Common.Attributes
{
    internal class DefaultIconLabelAttribute : Attribute
    {
        public string Icon { get; set; }
        public string Label { get; set; }
    }
}

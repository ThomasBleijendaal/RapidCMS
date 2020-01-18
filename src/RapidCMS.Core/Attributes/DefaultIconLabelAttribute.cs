using System;

namespace RapidCMS.Core.Attributes
{
    internal class DefaultIconLabelAttribute : Attribute
    {
        public DefaultIconLabelAttribute(string icon, string label)
        {
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            Label = label ?? throw new ArgumentNullException(nameof(label));
        }

        public string Icon { get; set; }
        public string Label { get; set; }
    }
}

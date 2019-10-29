using System;
using System.Collections.Generic;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.UI
{
    public class SectionUI
    {
        public SectionUI(Type? customType, string? label, Func<object, EntityState, bool> isVisible)
        {
            CustomType = customType;
            IsVisible = isVisible ?? throw new ArgumentNullException(nameof(isVisible));
            Label = label;
        }

        public List<ButtonUI>? Buttons { get; internal set; }

        public List<ElementUI>? Elements { get; internal set; }
        public Func<object, EntityState, bool> IsVisible { get; internal set; }
        public string? Label { get; internal set; }

        public Type? CustomType { get; internal set; }
    }
}

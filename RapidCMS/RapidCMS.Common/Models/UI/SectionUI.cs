using System;
using System.Collections.Generic;


namespace RapidCMS.Common.Models.UI
{
    public class SectionUI
    {
        public SectionUI(string? customAlias, string? label, Func<object, bool> isVisible)
        {
            CustomAlias = customAlias;
            IsVisible = isVisible ?? throw new ArgumentNullException(nameof(isVisible));
            Label = label;
        }

        public List<ButtonUI>? Buttons { get; internal set; }

        public List<ElementUI>? Elements { get; internal set; }
        public string? CustomAlias { get; internal set; }
        public Func<object, bool> IsVisible { get; internal set; }
        public string? Label { get; internal set; }
    }
}

using System;
using System.Collections.Generic;


namespace RapidCMS.Common.Models.UI
{
    public class SectionUI
    {
        public List<ButtonUI>? Buttons { get; internal set; }

        // TODO: should this be FieldUI instead?
        public List<ElementUI>? Elements { get; internal set; }
        public string? CustomAlias { get; internal set; }
        public Func<object, bool> IsVisible { get; internal set; }
        public string? Label { get; internal set; }
    }
}

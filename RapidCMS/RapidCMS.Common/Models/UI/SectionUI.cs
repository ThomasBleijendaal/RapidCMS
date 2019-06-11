using System.Collections.Generic;


namespace RapidCMS.Common.Models.UI
{
    public class SectionUI
    {
        public List<ButtonUI> Buttons { get; internal set; }
        public List<Element> Elements { get; internal set; }
        public string? CustomAlias { get; internal set; }
        public string? Label { get; internal set; }
    }
}

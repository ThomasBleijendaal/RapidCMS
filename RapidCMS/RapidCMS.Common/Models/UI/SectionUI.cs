using System.Collections.Generic;

#nullable enable

namespace RapidCMS.Common.Models.UI
{
    public class SectionUI
    {
        public List<ButtonUI> Buttons { get; set; }
        public List<Element> Elements { get; set; }
        public string? CustomAlias { get; set; }
    }
}

using System.Collections.Generic;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.UI
{
    // TODO: rename to NodeUI
    public class NodeUI
    {
        public UISubject Subject { get; set; }

        public List<ButtonUI> Buttons { get; set; }
        public List<SectionUI> Sections { get; set; }
    }
}

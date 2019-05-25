using System.Collections.Generic;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models.UI
{
    // TODO: move back to internal sets when weird Collection.razor NodeUI creation is no longer required

    public class NodeUI
    {
        public UISubject Subject { get; set; }

        public List<ButtonUI> Buttons { get; set; }
        public List<SectionUI> Sections { get; set; }
    }
}

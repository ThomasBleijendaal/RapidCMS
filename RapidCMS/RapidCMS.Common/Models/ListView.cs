using System.Collections.Generic;

namespace RapidCMS.Common.Models
{
    internal class ListView
    {
        internal int? PageSize { get; set; }
        internal bool? SearchBarVisible { get; set; }
        internal List<Pane>? ViewPanes { get; set; }
        internal List<Button>? Buttons { get; set; }
    }
}

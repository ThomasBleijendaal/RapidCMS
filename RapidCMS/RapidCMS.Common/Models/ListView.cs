using System.Collections.Generic;


namespace RapidCMS.Common.Models
{
    // TODO: investigate combining with ListEditor
    internal class ListView
    {
        internal ViewPane? ViewPane { get; set; }
        internal List<Button>? Buttons { get; set; }
    }
}

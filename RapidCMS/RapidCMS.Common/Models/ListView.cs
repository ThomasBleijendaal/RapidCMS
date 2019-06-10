using System.Collections.Generic;

#nullable enable

namespace RapidCMS.Common.Models
{
    // TODO: investigate combining with ListEditor
    internal class ListView
    {
        internal ViewPane? ViewPane { get; set; }
        internal List<Button>? Buttons { get; set; }
    }
}

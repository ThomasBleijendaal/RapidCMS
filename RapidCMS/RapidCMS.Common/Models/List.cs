using System.Collections.Generic;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models
{
    internal class List
    {
        internal int? PageSize { get; set; }
        internal bool? SearchBarVisible { get; set; }
        internal ListType ListType { get; set; }
        internal EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
        internal List<Pane>? Panes { get; set; }
        internal List<Button>? Buttons { get; set; }
    }
}

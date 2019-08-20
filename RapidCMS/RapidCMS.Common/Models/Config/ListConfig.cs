using System.Collections.Generic;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public class ListConfig
    {
        internal int? PageSize { get; set; }
        internal bool? SearchBarVisible { get; set; }
        internal ListType ListEditorType { get; set; }
        internal EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<PaneConfig> Panes { get; set; } = new List<PaneConfig>();
    }
}

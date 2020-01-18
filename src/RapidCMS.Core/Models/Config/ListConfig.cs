using System.Collections.Generic;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Config
{
    internal class ListConfig
    {
        internal int? PageSize { get; set; }
        internal bool? SearchBarVisible { get; set; }
        internal bool? ReorderingAllowed { get; set; }
        internal ListType ListEditorType { get; set; }
        internal EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
        internal List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        internal List<PaneConfig> Panes { get; set; } = new List<PaneConfig>();
    }
}

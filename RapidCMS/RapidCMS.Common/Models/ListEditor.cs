using System.Collections.Generic;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models
{
    internal class ListEditor
    {
        internal int? PageSize { get; set; }
        internal bool? SearchBarVisible { get; set; }
        internal ListEditorType ListEditorType { get; set; }
        internal EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
        internal List<Pane>? EditorPanes { get; set; }
        internal List<Button>? Buttons { get; set; }
    }
}

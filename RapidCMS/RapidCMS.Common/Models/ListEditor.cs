using System.Collections.Generic;
using RapidCMS.Common.Enums;


namespace RapidCMS.Common.Models
{
    // TODO: investigate combining with ListView
    internal class ListEditor
    {
        internal ListEditorType ListEditorType { get; set; }
        internal List<Pane>? EditorPanes { get; set; }
        internal List<Button>? Buttons { get; set; }
    }
}

using System.Collections.Generic;
using RapidCMS.Common.Enums;

#nullable enable

namespace RapidCMS.Common.Models
{
    internal class ListEditor
    {
        internal ListEditorType ListEditorType { get; set; }
        internal List<Pane> EditorPanes { get; set; }
        internal List<Button> Buttons { get; set; }
    }
}

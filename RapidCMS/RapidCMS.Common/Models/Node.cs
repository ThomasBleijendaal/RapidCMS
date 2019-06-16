using System;
using System.Collections.Generic;


namespace RapidCMS.Common.Models
{
    internal class Node
    {
        internal Type BaseType { get; set; }
        internal List<Pane>? EditorPanes { get; set; }
        internal List<Button>? Buttons { get; set; }
    }
}

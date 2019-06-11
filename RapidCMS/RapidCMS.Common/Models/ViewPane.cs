using System;
using System.Collections.Generic;


namespace RapidCMS.Common.Models
{
    [Obsolete]
    internal class ViewPane
    {
        internal string Name { get; set; }

        internal List<Field> Fields { get; set; }
        internal List<Button> Buttons { get; set; }
    }
}

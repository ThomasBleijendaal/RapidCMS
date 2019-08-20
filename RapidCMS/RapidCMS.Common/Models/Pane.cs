using System;
using System.Collections.Generic;

namespace RapidCMS.Common.Models
{
    internal class Pane
    {
        internal string? CustomAlias { get; set; }
        internal string? Label { get; set; }
        internal Func<object, bool> IsVisible { get; set; }
        internal Type VariantType { get; set; }
        internal List<Button> Buttons { get; set; }
        internal List<Field> Fields { get; set; }
        internal List<SubCollectionList> SubCollectionLists { get; set; }
        internal List<RelatedCollectionList> RelatedCollectionLists { get; set; }
    }
}

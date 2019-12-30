using System;
using System.Collections.Generic;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup
{
    public sealed class PaneSetup
    {
        internal Type? CustomType { get; set; }
        internal string? Label { get; set; }
        internal Func<object, EntityState, bool> IsVisible { get; set; }
        internal Type VariantType { get; set; }
        internal List<ButtonSetup> Buttons { get; set; }
        internal List<FieldSetup> Fields { get; set; }
        internal List<SubCollectionListSetup> SubCollectionLists { get; set; }
        internal List<RelatedCollectionListSetup> RelatedCollectionLists { get; set; }
    }
}

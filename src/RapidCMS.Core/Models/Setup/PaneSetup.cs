using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class PaneSetup
    {
        internal PaneSetup(PaneConfig pane)
        {
            CustomType = pane.CustomType;
            IsVisible = pane.IsVisible;
            Label = pane.Label;
            VariantType = pane.VariantType;
            Buttons = pane.Buttons.ToList(button => new ButtonSetup(button));
            Fields = pane.Fields.ToList(x => ConfigProcessingHelper.ProcessField(x));
            SubCollectionLists = pane.SubCollectionLists.ToList(x => new SubCollectionListSetup(x));
            RelatedCollectionLists = pane.RelatedCollectionLists.ToList(x => new RelatedCollectionListSetup(x));
        }

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

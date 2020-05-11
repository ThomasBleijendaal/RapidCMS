using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup
{
    internal class PaneSetup
    {
        public PaneSetup(
            Type? customType, 
            string? label, 
            Func<object, EntityState, bool> isVisible, 
            Type variantType, 
            List<IButtonSetup> buttons, 
            List<FieldSetup> fields, 
            List<SubCollectionListSetup> subCollectionLists, 
            List<RelatedCollectionListSetup> relatedCollectionLists)
        {
            CustomType = customType;
            Label = label;
            IsVisible = isVisible ?? throw new ArgumentNullException(nameof(isVisible));
            VariantType = variantType ?? throw new ArgumentNullException(nameof(variantType));
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));
            SubCollectionLists = subCollectionLists ?? throw new ArgumentNullException(nameof(subCollectionLists));
            RelatedCollectionLists = relatedCollectionLists ?? throw new ArgumentNullException(nameof(relatedCollectionLists));
        }

        internal Type? CustomType { get; set; }
        internal string? Label { get; set; }
        internal Func<object, EntityState, bool> IsVisible { get; set; }
        internal Type VariantType { get; set; }
        internal List<IButtonSetup> Buttons { get; set; }
        internal List<FieldSetup> Fields { get; set; }
        internal List<SubCollectionListSetup> SubCollectionLists { get; set; }
        internal List<RelatedCollectionListSetup> RelatedCollectionLists { get; set; }
    }
}

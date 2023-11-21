using System;
using System.Collections.Generic;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup;

public class PaneSetup
{
    public PaneSetup(
        Type? customType, 
        string? label, 
        Func<object, EntityState, bool> isVisible, 
        Type variantType, 
        List<ButtonSetup> buttons, 
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

    public Type? CustomType { get; set; }
    public string? Label { get; set; }
    public Func<object, EntityState, bool> IsVisible { get; set; }
    public Type VariantType { get; set; }
    public List<ButtonSetup> Buttons { get; set; }
    public List<FieldSetup> Fields { get; set; }
    public List<SubCollectionListSetup> SubCollectionLists { get; set; }
    public List<RelatedCollectionListSetup> RelatedCollectionLists { get; set; }
}

using System;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup;

public class ExpressionFieldSetup : FieldSetup
{
    internal ExpressionFieldSetup(FieldConfig field, IExpressionMetadata expression) : base(field)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        DisplayType = field.DisplayType;
    }

    internal ExpressionFieldSetup(FieldConfig field, IPropertyMetadata expression) : base(field)
    {
        Expression = PropertyMetadataHelper.GetExpressionMetadata(expression ?? throw new ArgumentNullException(nameof(expression)));
        DisplayType = field.DisplayType;
    }

    public ExpressionFieldSetup() : base(default)
    {

    }

    public DisplayType DisplayType { get; set; } = DisplayType.Label;
}

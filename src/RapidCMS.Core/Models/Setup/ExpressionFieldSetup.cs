using System;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class ExpressionFieldSetup : FieldSetup
    {
        internal ExpressionFieldSetup(FieldConfig field, IExpressionMetadata expression) : base(field)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        internal ExpressionFieldSetup(FieldConfig field, IPropertyMetadata expression) : base(field)
        {
            Expression = PropertyMetadataHelper.GetExpressionMetadata(expression ?? throw new ArgumentNullException(nameof(expression)));
        }
        
        internal DisplayType DisplayType { get; set; } = DisplayType.Label;
    }
}

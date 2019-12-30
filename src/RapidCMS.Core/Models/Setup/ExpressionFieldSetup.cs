using System;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Models.Setup
{
    internal class ExpressionFieldSetup : FieldSetup
    {
        public ExpressionFieldSetup(IExpressionMetadata expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public ExpressionFieldSetup(IPropertyMetadata expression)
        {
            Expression = PropertyMetadataHelper.GetExpressionMetadata(expression ?? throw new ArgumentNullException(nameof(expression)));
        }
        
        internal DisplayType DisplayType { get; set; } = DisplayType.Label;
        internal IExpressionMetadata Expression { get; set; }
    }
}

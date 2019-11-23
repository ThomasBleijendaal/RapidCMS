using System;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class CustomExpressionField : ExpressionField
    {
        public CustomExpressionField(IExpressionMetadata expression, Type customFieldType) : base(expression)
        {
            CustomType = customFieldType ?? throw new ArgumentNullException(nameof(customFieldType));
        }

        internal Type CustomType { get; set; }
    }
}

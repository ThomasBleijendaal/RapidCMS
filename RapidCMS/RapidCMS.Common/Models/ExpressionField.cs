using System;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class ExpressionField : Field
    {
        public ExpressionField(IExpressionMetadata expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        internal IExpressionMetadata Expression { get; set; }
    }
}

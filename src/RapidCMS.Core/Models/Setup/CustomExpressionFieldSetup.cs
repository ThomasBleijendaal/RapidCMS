using System;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Models.Setup
{
    internal class CustomExpressionFieldSetup : ExpressionFieldSetup
    {
        public CustomExpressionFieldSetup(IExpressionMetadata expression, Type customFieldType) : base(expression)
        {
            CustomType = customFieldType ?? throw new ArgumentNullException(nameof(customFieldType));
        }

        internal Type CustomType { get; set; }
    }
}

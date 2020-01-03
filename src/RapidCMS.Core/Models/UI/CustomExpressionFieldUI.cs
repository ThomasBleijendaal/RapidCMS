using System;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class CustomExpressionFieldUI : ExpressionFieldUI
    {
        internal CustomExpressionFieldUI(CustomExpressionFieldSetup field) : base(field)
        {
            CustomType = field.CustomType;
        }

        public Type CustomType { get; private set; }
    }
}

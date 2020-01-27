using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class ExpressionFieldUI : FieldUI
    {
        internal ExpressionFieldUI(ExpressionFieldSetup field) : base(field)
        {
            Expression = field.Expression;
            Type = field.DisplayType;
        }

        public DisplayType Type { get; private set; }
    }
}

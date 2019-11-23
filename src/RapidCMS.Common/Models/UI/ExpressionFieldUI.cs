using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.UI
{
    public class ExpressionFieldUI : FieldUI
    {
        public DisplayType Type { get; internal set; }
        public IExpressionMetadata Expression { get; internal set; }
    }
}

using RapidCMS.Common.Models.Metadata;

#nullable enable

namespace RapidCMS.Common.Models
{
    internal class ExpressionField : Field
    {
        internal IExpressionMetadata Expression { get; set; }
    }
}

using RapidCMS.Common.Models.Metadata;


namespace RapidCMS.Common.Models
{
    internal class ExpressionField : Field
    {
        internal IExpressionMetadata Expression { get; set; }
    }
}

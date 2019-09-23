using System;

namespace RapidCMS.Common.Models.Metadata
{
    internal class ExpressionMetadata : IExpressionMetadata
    {
        public string PropertyName { get; internal set; }
        public Func<object, string> StringGetter { get; internal set; }
    }
}

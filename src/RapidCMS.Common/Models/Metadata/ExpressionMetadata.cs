using System;

namespace RapidCMS.Common.Models.Metadata
{
    internal class ExpressionMetadata : IExpressionMetadata
    {
        public ExpressionMetadata(string propertyName, Func<object, string> stringGetter)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            StringGetter = stringGetter ?? throw new ArgumentNullException(nameof(stringGetter));
        }

        public string PropertyName { get; private set; }
        public Func<object, string> StringGetter { get; private set; }
    }
}

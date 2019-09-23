using System;

namespace RapidCMS.Common.Models.Metadata
{
    public interface IExpressionMetadata
    {
        string PropertyName { get; }
        Func<object, string> StringGetter { get; }
    }
}

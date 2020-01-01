using System;

namespace RapidCMS.Core.Abstractions.Metadata
{
    public interface IExpressionMetadata
    {
        string PropertyName { get; }
        Func<object, string> StringGetter { get; }
    }
}

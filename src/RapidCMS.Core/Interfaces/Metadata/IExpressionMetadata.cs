using System;

namespace RapidCMS.Core.Interfaces.Metadata
{
    public interface IExpressionMetadata
    {
        string PropertyName { get; }
        Func<object, string> StringGetter { get; }
    }
}

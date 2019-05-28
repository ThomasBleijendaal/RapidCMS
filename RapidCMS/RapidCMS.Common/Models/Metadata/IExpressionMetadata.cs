using System;

namespace RapidCMS.Common.Models.Metadata
{
    public interface IExpressionMetadata
    {
        Type PropertyType { get; }
        string PropertyName { get; }
        Func<object, string> StringGetter { get; }
    }
}

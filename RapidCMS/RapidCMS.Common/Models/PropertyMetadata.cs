using System;

namespace RapidCMS.Common.Models
{
    internal class PropertyMetadata : ExpressionMetadata, IPropertyMetadata
    {
        public string PropertyName { get; set; }
        public Action<object, object> Setter { get; set; }
    }

    internal class ExpressionMetadata : IExpressionMetadata
    {
        public Type ObjectType { get; set; }
        public Type PropertyType { get; set; }
        public Func<object, object> Getter { get; set; }
    }

    public interface IExpressionMetadata
    {
        Type ObjectType { get; }
        Type PropertyType { get; }
        Func<object, object> Getter { get; }
    }

    public interface IPropertyMetadata : IExpressionMetadata
    {
        string PropertyName { get; }
        Action<object, object> Setter { get; set; }
    }

}

using System;

namespace RapidCMS.Common.Exceptions
{
    public class InvalidExpressionException : Exception
    {
        public InvalidExpressionException(string attribute) : base($"Cannot process expression in {attribute} to Func<object, string> (StringGetter).")
        {
        }
    }

    public class InvalidPropertyExpressionException : Exception
    {
        public InvalidPropertyExpressionException(string attribute) : base($"Cannot process property expression in {attribute} to Func<object, object> and Action<object, object> (Getter and Setter).")
        {
        }
    }
}

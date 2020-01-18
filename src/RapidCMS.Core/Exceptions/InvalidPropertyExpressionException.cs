using System;

namespace RapidCMS.Core.Exceptions
{
    public class InvalidPropertyExpressionException : Exception
    {
        public InvalidPropertyExpressionException(string attribute) : base($"Cannot process property expression in {attribute} to Func<object, object> and Action<object, object> (Getter and Setter).")
        {
        }
    }
}

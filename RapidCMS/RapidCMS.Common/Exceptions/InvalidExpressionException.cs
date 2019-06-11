using System;

namespace RapidCMS.Common.Exceptions
{
    public class InvalidExpressionException : Exception
    {
        public InvalidExpressionException(string attribute) : base($"Cannot process expression in {attribute} to Func<object, string> (StringGetter).")
        {
        }
    }
}

using System;

namespace RapidCMS.Core.Exceptions;

public class NotUniqueException : Exception
{
    public NotUniqueException(string attribute) : base($"Value of {attribute} is not globally unique in this context.")
    {

    }
}

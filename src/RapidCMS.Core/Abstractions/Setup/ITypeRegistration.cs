using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface ITypeRegistration
    {
        Type Type { get; }
        string Alias { get; }
        Dictionary<string, object>? Parameters { get; }
    }
}

using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Models.Config;

public sealed class CustomTypeRegistrationConfig 
{
    internal CustomTypeRegistrationConfig(Type type, Dictionary<string, object>? parameters = null)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Alias = type.FullName ?? throw new InvalidOperationException($"The given type ({type}) must have a FullName");
        Parameters = parameters;
    }

    public Type Type { get; set; }
    public string Alias { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
}

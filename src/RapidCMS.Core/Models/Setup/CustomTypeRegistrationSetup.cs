using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Models.Setup
{
    internal class CustomTypeRegistrationSetup : ITypeRegistration
    {
        internal Type Type { get; set; } = default!;
        internal string Alias { get; set; } = default!;
        internal Dictionary<string, object>? Parameters { get; set; } = default!;

        Type ITypeRegistration.Type => Type;
        string ITypeRegistration.Alias => Alias;
        Dictionary<string, object>? ITypeRegistration.Parameters => Parameters;
    }
}

using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class CustomTypeRegistrationSetup : ITypeRegistration
    {
        internal CustomTypeRegistrationSetup(CustomTypeRegistrationConfig registration)
        {
            Type = registration.Type == typeof(CollectionConfig) ? typeof(CollectionSetup) : registration.Type;
            Alias = registration.Alias;
            Parameters = registration.Parameters;
        }

        internal Type Type { get; set; }
        internal string Alias { get; set; }
        internal Dictionary<string, object>? Parameters { get; set; }

        Type ITypeRegistration.Type => Type;
        string ITypeRegistration.Alias => Alias;
        Dictionary<string, object>? ITypeRegistration.Parameters => Parameters;
    }
}

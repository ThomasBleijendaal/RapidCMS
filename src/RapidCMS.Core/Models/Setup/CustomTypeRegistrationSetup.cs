using System;
using System.Collections.Generic;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class CustomTypeRegistrationSetup
    {
        internal CustomTypeRegistrationSetup(CustomTypeRegistrationConfig registration)
        {
            Type = registration.Type == typeof(CollectionConfig) ? typeof(CollectionSetup) : registration.Type;
            Alias = registration.Alias;
            Parameters = registration.Parameters;
        }

        internal Type Type { get; set; }
        internal string Alias { get; set; }
        internal Dictionary<string, string>? Parameters { get; set; }
    }
}

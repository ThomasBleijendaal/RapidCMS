using System;
using System.Collections.Generic;

namespace RapidCMS.Common.Models
{
    public class CustomTypeRegistration
    {
        internal CustomTypeRegistration(Type type, Dictionary<string, string>? parameters = null)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Alias = type.FullName ?? throw new InvalidOperationException($"The given type ({type}) must have a FullName");
            ;
            Parameters = parameters;
        }

        public Type Type { get; set; }
        public string Alias { get; set; }
        public Dictionary<string, string>? Parameters { get; set; }
    }
}

using System;

namespace RapidCMS.Core.Models.Setup
{
    public class ValidationSetup
    {
        public ValidationSetup(Type type, object? configuration)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Configuration = configuration;
        }

        public Type Type { get; set; }
        public object? Configuration { get; set; }
    }
}

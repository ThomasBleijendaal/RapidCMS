using System;

namespace RapidCMS.Core.Models.Config
{
    internal class ValidationConfig
    {
        public ValidationConfig(Type type, object? configuration)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Configuration = configuration;
        }

        public Type Type { get; set; }
        public object? Configuration { get; set; }
    }
}

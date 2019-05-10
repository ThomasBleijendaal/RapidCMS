using RapidCMS.Common.Enums;
using System;

#nullable enable

namespace RapidCMS.Common.Models.Config
{
    public class CustomButtonConfig : ButtonConfig
    {
        public CustomButtonConfig(Type customButtonType)
        {
            Alias = customButtonType?.FullName ?? throw new ArgumentNullException(nameof(customButtonType));
        }

        internal string Alias { get; set; }
        internal CrudType CrudType { get; set; }
        internal Action? Action { get; set; } = null;
        internal Type? ActionHandler { get; set; } = null;
    }
}

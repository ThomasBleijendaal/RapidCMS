using System;

#nullable enable

namespace RapidCMS.Common.Models.Config
{
    public class CustomButtonConfig : ButtonConfig
    {
        public CustomButtonConfig(string alias)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        internal string Alias { get; set; }
        internal Action? Action { get; set; } = null;
        internal Type? ActionHandler { get; set; } = null;
    }
}

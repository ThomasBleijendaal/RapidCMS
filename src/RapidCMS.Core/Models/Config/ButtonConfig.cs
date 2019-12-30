using System;

namespace RapidCMS.Core.Models.Config
{
    internal class ButtonConfig
    {
        internal string Id { get; set; } = Guid.NewGuid().ToString();
        internal string? Label { get; set; }
        internal string? Icon { get; set; }
        internal bool IsPrimary { get; set; }
    }
}
